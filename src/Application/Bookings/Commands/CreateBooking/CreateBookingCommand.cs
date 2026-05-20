using Heven.Api.Application.Common.Exceptions;
using Heven.Api.Application.Common.Interfaces;
using Heven.Api.Application.Common.Security;
using Heven.Api.Domain.Constants;
using Heven.Api.Domain.Entities;
using Heven.Api.Domain.Enums;
using Heven.Api.Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Heven.Api.Application.Bookings.Commands.CreateBooking;

[Authorize]
public record CreateBookingCommand : IRequest<int>
{
    public int ListingId { get; init; }
    public DateOnly CheckIn { get; init; }
    public DateOnly CheckOut { get; init; }
    public int GuestCount { get; init; }
    public string? SpecialRequests { get; init; }
}

public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;
    private readonly IMediator _mediator;
    private readonly IDistributedLockService _lockService;
    private readonly IIdentityService _identityService;
    private readonly IBookingJobService _bookingJobService;

    public CreateBookingCommandHandler(
        IApplicationDbContext context,
        IUser user,
        IMediator mediator,
        IDistributedLockService lockService,
        IIdentityService identityService,
        IBookingJobService bookingJobService)
    {
        _context = context;
        _user = user;
        _mediator = mediator;
        _lockService = lockService;
        _identityService = identityService;
        _bookingJobService = bookingJobService;
    }

    public async Task<int> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        // 1. Kiểm tra User Profile và Email Verification
        var hasProfile = await _context.UserProfiles.AnyAsync(p => p.UserId == _user.Id!, cancellationToken);
        if (!hasProfile)
        {
            throw new ForbiddenAccessException("You need to complete your profile before booking.", ErrorCodes.User.ProfileIncomplete);
        }

        var hasVerifiedEmail = await _identityService.IsEmailConfirmedAsync(_user.Id!);
        if (!hasVerifiedEmail)
        {
            throw new ForbiddenAccessException("Verify your email before booking.", ErrorCodes.User.EmailNotVerified);
        }

        // 2. Logic kiểm tra Listing và phân quyền
        var listing = await _context.Listings.FirstOrDefaultAsync(l => l.Id == request.ListingId, cancellationToken);
        if (listing == null)
        {

            throw new NotFoundException("Listing not found.", ErrorCodes.Listing.NotFound);
        }

        if (listing.Status != ListingStatus.Active)
        {
            throw new ForbiddenAccessException("This listing is not available for booking.", ErrorCodes.Listing.NotAvailable);
        }

        if (listing.HostId == _user.Id!)
        {
            throw new ConflictException("You cannot book your own listing.", ErrorCodes.Booking.OwnListing);
        }

        if (request.GuestCount > listing.MaxGuests)
        {
            throw new ConflictException($"Guest count cannot exceed {listing.MaxGuests}.", ErrorCodes.Booking.InvalidGuestCount);
        }

        // 3. Xử lý Lock và Calendar
        await using var distributedLock = await _lockService.AcquireAsync($"listing:{request.ListingId}", cancellationToken);
        if (!distributedLock.IsAcquired)
        {
            throw new ConflictException("The listing is currently being processed by another booking request. Please try again.", ErrorCodes.Booking.ListingLocked);
        }

        var stayDates = new List<DateOnly>();
        for (var d = request.CheckIn; d < request.CheckOut; d = d.AddDays(1))
        {
            stayDates.Add(d);
        }

        var calendarRows = await _context.ListingCalendars
            .Where(lc => lc.ListingId == request.ListingId && stayDates.Contains(lc.Date))
            .ToListAsync(cancellationToken);

        // -- Kiểm tra Calendar -- //
        if (calendarRows.Count != stayDates.Count)
        {
            throw new ConflictException("One or more nights are not configured on the listing calendar.", ErrorCodes.Booking.DatesNotConfigured);
        }

        if (calendarRows.Any(r => r.Status != ListingCalendarStatus.Available))
        {
            throw new ConflictException("Some selected dates are blocked or already booked.", ErrorCodes.Booking.DatesUnavailable);
        }
        // ------------------------------------ //

        decimal nightlyTotal = 0;
        foreach (var row in calendarRows)
        {
            nightlyTotal += row.Price ?? listing.PricePerNight;
        }

        var totalPrice = nightlyTotal + listing.CleaningFee;

        var booking = new Booking
        {
            ListingId = listing.Id,
            GuestId = _user.Id!,
            CheckIn = request.CheckIn,
            CheckOut = request.CheckOut,
            GuestCount = request.GuestCount,
            TotalPrice = totalPrice,
            Status = BookingStatus.Pending,
            SpecialRequests = request.SpecialRequests
        };

        booking.AddDomainEvent(new BookingCreatedEvent(booking));

        _context.Bookings.Add(booking);
        
        await _context.SaveChangesAsync(cancellationToken);

        // Schedule timeout job after saving to get the booking ID
        var jobId = _bookingJobService.SchedulePaymentTimeout(booking.Id, TimeSpan.FromMinutes(10));
        booking.TimeoutJobId = jobId;
        await _context.SaveChangesAsync(cancellationToken);

        return booking.Id;
    }
}

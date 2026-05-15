using Heven.Api.Application.Common.Interfaces;
using Heven.Api.Application.Common.Security;
using Heven.Api.Domain.Constants;
using Heven.Api.Domain.Entities;
using Heven.Api.Domain.Enums;
using Heven.Api.Domain.Events;

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

    public CreateBookingCommandHandler(
        IApplicationDbContext context,
        IUser user,
        IMediator mediator,
        IDistributedLockService lockService)
    {
        _context = context;
        _user = user;
        _mediator = mediator;
        _lockService = lockService;
    }

    public async Task<int> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        await using var distributedLock = await _lockService.AcquireAsync($"listing:{request.ListingId}", cancellationToken);
        if (!distributedLock.IsAcquired)
        {
            throw new InvalidOperationException("The listing is currently being processed by another booking request. Please try again.");
        }

        var listing = await _context.Listings.FirstOrDefaultAsync(l => l.Id == request.ListingId, cancellationToken);
        Guard.Against.NotFound(request.ListingId, listing);

        var stayDates = new List<DateOnly>();
        for (var d = request.CheckIn; d < request.CheckOut; d = d.AddDays(1))
        {
            stayDates.Add(d);
        }

        var calendarRows = await _context.ListingCalendars
            .Where(lc => lc.ListingId == request.ListingId && stayDates.Contains(lc.Date))
            .ToListAsync(cancellationToken);

        // -- Check CALENDAR -- //
        if (calendarRows.Count != stayDates.Count)
        {
            throw new InvalidOperationException("One or more nights are not configured on the listing calendar.");
        }

        if (calendarRows.Any(r => r.Status != ListingCalendarStatuses.Available))
        {
            throw new InvalidOperationException("Some selected dates are blocked or already booked.");
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

        await _mediator.Publish(new BookingPendingCreatedEvent(booking), cancellationToken);

        return booking.Id;
    }
}

using Heven.Api.Application.Common.Exceptions;
using Heven.Api.Application.Common.Interfaces;
using Heven.Api.Application.Common.Security;
using Heven.Api.Domain.Constants;
using Heven.Api.Domain.Enums;
using Heven.Api.Domain.Events;
using MediatR;

namespace Heven.Api.Application.Bookings.Commands.CancelBooking;

[Authorize(Roles = $"{Roles.Guest},{Roles.Host},{Roles.Administrator}")]
public record CancelBookingCommand(int BookingId) : IRequest;

public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public CancelBookingCommandHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings.FindAsync([request.BookingId], cancellationToken);

        if (booking == null)
        {
            throw new NotFoundException($"Booking not found.", ErrorCodes.Booking.NotFound);
        }

        // Đảm bảo chỉ người tạo Booking mới được hủy (hoặc là Admin/Host tùy nghiệp vụ)
        if (booking.GuestId != _user.Id)
        {
            throw new ForbiddenAccessException("You are not allowed to cancel this booking.");
        }

        if (booking.Status == BookingStatus.Cancelled)
        {
            throw new ConflictException("Booking is already cancelled.");
        }

        // Logic nghiệp vụ: chỉ cho phép hủy khi đang Pending hoặc Confirmed (tuỳ policy)
        if (booking.Status == BookingStatus.Completed)
        {
            throw new ConflictException("Cannot cancel a completed booking.");
        }

        booking.Status = BookingStatus.Cancelled;
        booking.AddDomainEvent(new BookingCancelledEvent(booking));

        await _context.SaveChangesAsync(cancellationToken);
    }
}

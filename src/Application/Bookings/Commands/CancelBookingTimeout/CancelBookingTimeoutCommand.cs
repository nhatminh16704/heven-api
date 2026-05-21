using Heven.Api.Application.Common.Interfaces;
using Heven.Api.Domain.Enums;
using Heven.Api.Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Heven.Api.Application.Bookings.Commands.CancelBookingTimeout;

public record CancelBookingTimeoutCommand(int BookingId) : IRequest;

public class CancelBookingTimeoutCommandHandler : IRequestHandler<CancelBookingTimeoutCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CancelBookingTimeoutCommandHandler> _logger;

    public CancelBookingTimeoutCommandHandler(IApplicationDbContext context, ILogger<CancelBookingTimeoutCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(CancelBookingTimeoutCommand request, CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == request.BookingId, cancellationToken);

        if (booking == null)
        {
            _logger.LogWarning("Hangfire Timeout Job: Booking {BookingId} not found.", request.BookingId);
            return;
        }

        if (booking.Status == BookingStatus.Pending)
        {
            booking.Status = BookingStatus.Cancelled;
            booking.AddDomainEvent(new BookingCancelledEvent(booking));

            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Booking {BookingId} has been cancelled by Hangfire due to payment timeout.", request.BookingId);
        }
        else
        {
            _logger.LogInformation("Booking {BookingId} is not in Pending state ({Status}). No action taken.", request.BookingId, booking.Status);
        }
    }
}

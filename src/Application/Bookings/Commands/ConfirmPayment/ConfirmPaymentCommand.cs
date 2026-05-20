using Ardalis.GuardClauses;
using Heven.Api.Application.Common.Interfaces;
using MediatR;
using Heven.Api.Domain.Enums;

namespace Heven.Api.Application.Bookings.Commands.ConfirmPayment;

public record ConfirmPaymentCommand(int BookingId) : IRequest;

public class ConfirmPaymentCommandHandler : IRequestHandler<ConfirmPaymentCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IBookingJobService _bookingJobService;

    public ConfirmPaymentCommandHandler(IApplicationDbContext context, IBookingJobService bookingJobService)
    {
        _context = context;
        _bookingJobService = bookingJobService;
    }

    public async Task Handle(ConfirmPaymentCommand request, CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings.FindAsync([request.BookingId], cancellationToken);

        Guard.Against.NotFound(request.BookingId, booking);

        booking.Status = BookingStatus.Confirmed; 
        
        if (!string.IsNullOrEmpty(booking.TimeoutJobId))
        {
            _bookingJobService.CancelPaymentTimeout(booking.TimeoutJobId);
            booking.TimeoutJobId = null;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}

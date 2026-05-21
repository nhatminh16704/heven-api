using Ardalis.GuardClauses;
using Heven.Api.Application.Common.Interfaces;
using MediatR;
using Heven.Api.Domain.Enums;

namespace Heven.Api.Application.Bookings.Commands.ConfirmPayment;

public record ConfirmPaymentCommand(int BookingId, string TransactionId) : IRequest;

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

        if (booking.Status == BookingStatus.Confirmed) return;

        booking.Status = BookingStatus.Confirmed; 
        
        var payment = new Heven.Api.Domain.Entities.Payment
        {
            BookingId = booking.Id,
            Amount = booking.TotalPrice,
            Method = "stripe",
            Status = PaymentStatus.Completed,
            TransactionId = request.TransactionId,
            PaidAt = DateTimeOffset.UtcNow
        };
        
        _context.Payments.Add(payment);
        
        if (!string.IsNullOrEmpty(booking.TimeoutJobId))
        {
            _bookingJobService.CancelPaymentTimeout(booking.TimeoutJobId);
            booking.TimeoutJobId = null;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}

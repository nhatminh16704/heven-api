using Ardalis.GuardClauses;
using Heven.Api.Application.Common.Interfaces;
using MediatR;
using Heven.Api.Domain.Enums; // B?n hãy ??i namespace này n?u BookingStatus n?m ? th? m?c khác

namespace Heven.Api.Application.Bookings.Commands.ConfirmPayment;

public record ConfirmPaymentCommand(int BookingId) : IRequest;

public class ConfirmPaymentCommandHandler : IRequestHandler<ConfirmPaymentCommand>
{
    private readonly IApplicationDbContext _context;

    public ConfirmPaymentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ConfirmPaymentCommand request, CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings.FindAsync([request.BookingId], cancellationToken);

        Guard.Against.NotFound(request.BookingId, booking);

        booking.Status = BookingStatus.Confirmed; 

        await _context.SaveChangesAsync(cancellationToken);
    }
}

using Ardalis.GuardClauses;
using Heven.Api.Application.Common.Interfaces;
using MediatR;

namespace Heven.Api.Application.Bookings.Commands.CreateCheckoutSession;

public record CreateCheckoutSessionCommand : IRequest<string>
{
    public int BookingId { get; init; }
    public string SuccessUrl { get; init; } = string.Empty;
    public string CancelUrl { get; init; } = string.Empty;
}

public class CreateCheckoutSessionCommandHandler : IRequestHandler<CreateCheckoutSessionCommand, string>
{
    private readonly IApplicationDbContext _context;
    private readonly IPaymentService _paymentService;

    public CreateCheckoutSessionCommandHandler(IApplicationDbContext context, IPaymentService paymentService)
    {
        _context = context;
        _paymentService = paymentService;
    }

    public async Task<string> Handle(CreateCheckoutSessionCommand request, CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings.FindAsync([request.BookingId], cancellationToken);

        Guard.Against.NotFound(request.BookingId, booking);

        // Truyền dữ liệu về PaymentService
        var sessionUrl = await _paymentService.CreateCheckoutSessionAsync(
            amount: booking.TotalPrice,
            currency: "usd",
            productName: $"Payment for Booking ID: {booking.Id}",
            successUrl: request.SuccessUrl,
            cancelUrl: request.CancelUrl,
            clientReferenceId: booking.Id.ToString(),   
            cancellationToken: cancellationToken);

        return sessionUrl;
    }
}

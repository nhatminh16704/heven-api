using Heven.Api.Application.Common.Interfaces;
using Stripe.Checkout;

namespace Heven.Api.Infrastructure.Services;

public class StripePaymentService : IPaymentService
{
    public async Task<string> CreateCheckoutSessionAsync(
        decimal amount, 
        string currency, 
        string productName, 
        string successUrl, 
        string cancelUrl, 
        string clientReferenceId, 
        CancellationToken cancellationToken = default)
    {
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = ["card"],
            LineItems =
            [
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(amount * 100), // Quy đổi tiền tệ (cent)
                        Currency = currency.ToLower(),
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = productName
                        }
                    },
                    Quantity = 1,
                }
            ],
            Mode = "payment",
            SuccessUrl = successUrl,
            CancelUrl = cancelUrl,
            ClientReferenceId = clientReferenceId
        };

        var service = new SessionService();
        Session session = await service.CreateAsync(options, cancellationToken: cancellationToken);

        return session.Url;
    }
}

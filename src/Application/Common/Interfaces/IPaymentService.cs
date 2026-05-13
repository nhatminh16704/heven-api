namespace Heven.Api.Application.Common.Interfaces;

public interface IPaymentService
{
    Task<string> CreateCheckoutSessionAsync(
        decimal amount,
        string currency,
        string productName,
        string successUrl,
        string cancelUrl,
        string clientReferenceId,
        CancellationToken cancellationToken = default);
}

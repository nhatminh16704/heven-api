using System.Text.Json;
using Heven.Api.Application.Common.Caching.Keys;
using Heven.Api.Domain.Events;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Heven.Api.Application.Bookings.EventHandlers;

public class BookingPendingCreatedPaymentCacheHandler : INotificationHandler<BookingPendingCreatedEvent>
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    private readonly IDistributedCache _cache;
    private readonly ILogger<BookingPendingCreatedPaymentCacheHandler> _logger;

    public BookingPendingCreatedPaymentCacheHandler(
        IDistributedCache cache,
        ILogger<BookingPendingCreatedPaymentCacheHandler> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task Handle(BookingPendingCreatedEvent notification, CancellationToken cancellationToken)
    {
        var booking = notification.Booking;

        var payload = JsonSerializer.Serialize(
            new { booking.Id, booking.ListingId, PendingUntil = DateTimeOffset.UtcNow.AddMinutes(10) },
            JsonOptions);

        try
        {
            await _cache.SetStringAsync(
                BookingCacheKeys.PaymentHold(booking.Id),
                payload,
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) },
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to set payment-hold cache for booking {BookingId}", booking.Id);
        }
    }
}

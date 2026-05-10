using Heven.Api.Domain.Events;
using Heven.Api.Application.Common.Caching.Keys;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Heven.Api.Application.Listings.EventHandlers;

public class ListingUpdatedEventHandler : INotificationHandler<ListingUpdatedEvent>
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<ListingUpdatedEventHandler> _logger;

    public ListingUpdatedEventHandler(IDistributedCache cache, ILogger<ListingUpdatedEventHandler> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task Handle(ListingUpdatedEvent notification, CancellationToken cancellationToken)
    {
        var listingId = notification.Item.Id;
        
        var detailCacheKey = ListingCacheKeys.Details(listingId);
        
        await _cache.RemoveAsync(detailCacheKey, cancellationToken);

        _logger.LogInformation("Domain Event: Cache cleared for {CacheKey}", detailCacheKey);

        // Vì listing bị thay đổi (ví dụ: đổi giá, đổi tên), dữ liệu hiển thị ngoài trang chủ (List)
        // cũng có thể bị sai lệchsẽ cần xóa cả Cache của List.
    }
}
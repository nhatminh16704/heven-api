using System.Text.Json;
using Heven.Api.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Heven.Api.Application.Common.Behaviours;

public class CachingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
    where TRequest : IRequest<TResponse>, ICacheable // Chỉ áp dụng cho request ICacheable
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<CachingBehaviour<TRequest, TResponse>> _logger;

    public CachingBehaviour(IDistributedCache cache, ILogger<CachingBehaviour<TRequest, TResponse>> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // 1. Kiểm tra cache
        string? cachedResponse = await _cache.GetStringAsync(request.CacheKey, cancellationToken);
        if (!string.IsNullOrEmpty(cachedResponse))
        {
            _logger.LogInformation("Fetch data from Cache with Key: {CacheKey}", request.CacheKey);
            var deserializedData = JsonSerializer.Deserialize<TResponse>(cachedResponse);
            if (deserializedData != null)
                return deserializedData;
        }

        // 2. Nếu Cache Miss, chạy vào Handler để lấy dữ liệu thực tế từ DB
        var response = await next();

        // 3. Set dữ liệu xuống Cache cho lần sau
        var cacheOptions = new DistributedCacheEntryOptions();
        
        // Thiết lập Absolute Expiration
        if (request.Expiration.HasValue)
        {
            cacheOptions.AbsoluteExpirationRelativeToNow = request.Expiration;
        }
        else
        {
            cacheOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15); // Mặc định 15 phút
        }

        // Thiết lập Sliding Expiration nếu có
        if (request.SlidingExpiration.HasValue) 
        {
            cacheOptions.SlidingExpiration = request.SlidingExpiration;
        }

        var serializedData = JsonSerializer.Serialize(response);
        await _cache.SetStringAsync(request.CacheKey, serializedData, cacheOptions, cancellationToken);

        _logger.LogInformation("Setted caching data for Key: {CacheKey}", request.CacheKey);

        return response;
    }
}

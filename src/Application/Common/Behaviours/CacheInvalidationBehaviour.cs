using Heven.Api.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Heven.Api.Application.Common.Behaviours;

public class CacheInvalidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
    where TRequest : IRequest<TResponse>, ICacheInvalidator // Chỉ áp dụng cho request thực thi ICacheInvalidator
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<CacheInvalidationBehaviour<TRequest, TResponse>> _logger;

    public CacheInvalidationBehaviour(IDistributedCache cache, ILogger<CacheInvalidationBehaviour<TRequest, TResponse>> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // 1. Đợi Command chạy xong (Lưu dữ liệu vào DB thành công)
        var response = await next();

        // 2. Chạy vào tiến trình xóa Cache sau khi có DB Changes
        if (request.CacheKeys != null && request.CacheKeys.Length > 0)
        {
            foreach (var key in request.CacheKeys)
            {
                await _cache.RemoveAsync(key, cancellationToken);
                _logger.LogInformation("Invalidated cache for Key: {CacheKey}", key);
            }
        }

        return response;
    }
}

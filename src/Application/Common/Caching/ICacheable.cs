namespace Heven.Api.Application.Common.Caching;

public interface ICacheable
{
    string CacheKey { get; }

    TimeSpan? SlidingExpiration => null;

    TimeSpan? Expiration => null;
}

namespace Heven.Api.Application.Common.Interfaces;

public interface ICacheable
{
    string CacheKey { get; }
    
    TimeSpan? SlidingExpiration => null;
    
    TimeSpan? Expiration => null;
}

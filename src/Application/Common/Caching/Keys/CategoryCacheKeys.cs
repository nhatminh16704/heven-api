namespace Heven.Api.Application.Common.Caching.Keys;

public static class CategoryCacheKeys
{
    public static string Details(int id) => $"categories:{id}";
    public static string All => "categories:all";
}

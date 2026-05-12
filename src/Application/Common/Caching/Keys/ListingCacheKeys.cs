namespace Heven.Api.Application.Common.Caching.Keys;

public static class ListingCacheKeys
{
    public static string Details(int id) => $"listings:{id}";
    public static string List(int page, int pageSize) => $"listings:all:p{page}:s{pageSize}";
    public static string Reviews(int id, int page, int pageSize) => $"listings:{id}:reviews:p{page}:s{pageSize}";
}

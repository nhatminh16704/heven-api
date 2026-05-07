namespace Heven.Api.Application.Common.Caching.Keys;

public static class ListingCacheKeys
{
    public static string Details(int id) => $"Listing_Details_{id}";
    // public static string List(int page, int pageSize) => $"Listing_List_P{page}_S{pageSize}";
}

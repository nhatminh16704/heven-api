namespace Heven.Api.Application.Common.Caching.Keys;

public static class CategoryCacheKeys
{
    public static string Details(int id) => $"Category_Details_{id}";
    public static string All => "Category_All";
}

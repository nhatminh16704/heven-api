namespace Heven.Api.Application.Common.Caching.Keys;

public static class BookingCacheKeys
{
    public static string Details(int id) => $"bookings:{id}";
    public static string ByUser(string userId) => $"bookings:user:{userId}";
}

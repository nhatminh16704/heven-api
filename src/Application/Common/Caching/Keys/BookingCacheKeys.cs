namespace Heven.Api.Application.Common.Caching.Keys;

public static class BookingCacheKeys
{
    public static string Details(int id) => $"Booking_Details_{id}";
    public static string ByUser(string userId) => $"Booking_ByUser_{userId}";
}

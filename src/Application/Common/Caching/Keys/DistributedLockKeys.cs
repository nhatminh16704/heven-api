namespace Heven.Api.Application.Common.Caching.Keys;

public static class DistributedLockKeys
{
    public static string ListingBooking(int listingId) => $"locks:listing:{listingId}:booking";
}

namespace Heven.Api.Domain.Constants;

public static class ErrorCodes
{
    public static class User
    {
        private const string Prefix = "USER";

        public const string ProfileIncomplete = $"{Prefix}.PROFILE_INCOMPLETE";
        public const string EmailNotVerified = $"{Prefix}.EMAIL_NOT_VERIFIED";
    }

    public static class Review
    {
        private const string Prefix = "REVIEW";

        public const string BookingNotCompleted = $"{Prefix}.BOOKING_NOT_COMPLETED";
        public const string AlreadyReviewed = $"{Prefix}.ALREADY_REVIEWED";
    }

    public static class Booking
    {
        private const string Prefix = "BOOKING";

        public const string ListingLocked = $"{Prefix}.LISTING_LOCKED";
        public const string DatesNotConfigured = $"{Prefix}.DATES_NOT_CONFIGURED";
        public const string DatesUnavailable = $"{Prefix}.DATES_UNAVAILABLE";
    }
}

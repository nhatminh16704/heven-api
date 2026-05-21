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

        public const string NotFound = $"{Prefix}.NOT_FOUND";
        public const string BookingNotCompleted = $"{Prefix}.BOOKING_NOT_COMPLETED";
        public const string AlreadyReviewed = $"{Prefix}.ALREADY_REVIEWED";
        public const string AlreadyReplied = $"{Prefix}.ALREADY_REPLIED";
        public const string Forbidden = $"{Prefix}.FORBIDDEN";
    }

    public static class Booking
    {
        private const string Prefix = "BOOKING";

        public const string NotFound = $"{Prefix}.NOT_FOUND";
        public const string ListingLocked = $"{Prefix}.LISTING_LOCKED";
        public const string DatesNotConfigured = $"{Prefix}.DATES_NOT_CONFIGURED";
        public const string DatesUnavailable = $"{Prefix}.DATES_UNAVAILABLE";
        public const string OwnListing = $"{Prefix}.OWN_LISTING";
        public const string InvalidGuestCount = $"{Prefix}.INVALID_GUEST_COUNT";
    }

    public static class Listing
    {
        private const string Prefix = "LISTING";

        public const string NotFound = $"{Prefix}.NOT_FOUND";
        public const string NotAvailable = $"{Prefix}.NOT_AVAILABLE";
    }
}

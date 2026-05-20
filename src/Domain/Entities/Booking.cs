using Heven.Api.Domain.Common;
using Heven.Api.Domain.Enums;

namespace Heven.Api.Domain.Entities;

public class Booking : BaseAuditableEntity
{
    public int ListingId { get; set; }
    public required string GuestId { get; set; }

    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }
    public int GuestCount { get; set; }
    public decimal TotalPrice { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public string? SpecialRequests { get; set; }
    public string? TimeoutJobId { get; set; }

    // Navigation
    public Listing Listing { get; set; } = null!;
    public Payment? Payment { get; set; }
    public BookingCancellation? Cancellation { get; set; }
}

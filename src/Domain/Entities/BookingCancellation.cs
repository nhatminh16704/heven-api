using Heven.Api.Domain.Common;

namespace Heven.Api.Domain.Entities;

public class BookingCancellation : BaseEntity
{
    public int BookingId { get; set; }
    public required string CancelledBy { get; set; } 

    public required string Reason { get; set; }
    public decimal RefundAmount { get; set; }
    public string? RefundStatus { get; set; }
    public DateTimeOffset CancelledAt { get; set; } = DateTimeOffset.UtcNow;

    public Booking Booking { get; set; } = null!;
}

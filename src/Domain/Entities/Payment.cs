using Heven.Api.Domain.Common;
using Heven.Api.Domain.Enums;

namespace Heven.Api.Domain.Entities;

public class Payment : BaseEntity 
{
    public int BookingId { get; set; }

    public decimal Amount { get; set; }
    public required string Method { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string? TransactionId { get; set; }
    public decimal? RefundedAmount { get; set; }

    public DateTimeOffset? PaidAt { get; set; }
    public DateTimeOffset? RefundedAt { get; set; }

    public Booking Booking { get; set; } = null!;
}

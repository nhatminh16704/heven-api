using Heven.Api.Domain.Common;
using Heven.Api.Domain.Enums;

namespace Heven.Api.Domain.Entities;

public class Review : BaseAuditableEntity
{
    public int BookingId { get; set; }
    public int ListingId { get; set; }
    public required string AuthorId { get; set; }
    public required string TargetId { get; set; }

    public ReviewType Type { get; set; }
    public int OverallRating { get; set; }
    public string? Comment { get; set; }

    public Booking Booking { get; set; } = null!;
    public Listing Listing { get; set; } = null!;
}

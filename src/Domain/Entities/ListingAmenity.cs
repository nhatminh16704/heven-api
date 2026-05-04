using Heven.Api.Domain.Common;

namespace Heven.Api.Domain.Entities;

public class ListingAmenity : BaseEntity
{
    public int ListingId { get; set; }
    public required string Name { get; set; }
    public string? Icon { get; set; }
    public string? Category { get; set; }

    public Listing Listing { get; set; } = null!;
}

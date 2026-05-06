using Heven.Api.Domain.Common;

namespace Heven.Api.Domain.Entities;

public class ListingAmenity : BaseEntity
{
    public int ListingId { get; set; }
    public int AmenityId { get; set; }

    // Navigation properties
    public Listing Listing { get; set; } = null!;
    public Amenity Amenity { get; set; } = null!;
}

using Heven.Api.Domain.Common;

namespace Heven.Api.Domain.Entities;

public class Amenity : BaseEntity
{
    public required string Name { get; set; }
    public string? Icon { get; set; }
    public string? Category { get; set; }

    // Navigation property
    public ICollection<ListingAmenity> ListingAmenities { get; set; } = new List<ListingAmenity>();
}

using Heven.Api.Domain.Common;
using Heven.Api.Domain.Enums;

namespace Heven.Api.Domain.Entities;

public class Listing : BaseAuditableEntity
{
    public required string HostId { get; set; }
    public int CategoryId { get; set; }
    public int LocationId { get; set; }

    public required string Title { get; set; }
    public string? Description { get; set; }
    public decimal PricePerNight { get; set; }
    public decimal CleaningFee { get; set; }
    public int MaxGuests { get; set; }
    public int Bedrooms { get; set; }
    public int Beds { get; set; }
    public int Bathrooms { get; set; }
    public string? PropertyType { get; set; }
    public bool InstantBook { get; set; }
    public ListingStatus Status { get; set; } = ListingStatus.Pending;

    // Navigation Domain Properties
    public Category Category { get; set; } = null!;
    public Location Location { get; set; } = null!;

    public ICollection<ListingImage> Images { get; set; } = new List<ListingImage>();
    public ICollection<ListingAmenity> Amenities { get; set; } = new List<ListingAmenity>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}

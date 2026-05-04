using Heven.Api.Domain.Common;

namespace Heven.Api.Domain.Entities;

public class Location : BaseEntity
{
    public required string Address { get; set; }
    public required string City { get; set; }
    public required string State { get; set; }
    public required string Country { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public ICollection<Listing> Listings { get; set; } = new List<Listing>();
}

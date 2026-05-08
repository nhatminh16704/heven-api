using Heven.Api.Domain.Common;

namespace Heven.Api.Domain.Entities;

public class Location : BaseEntity
{
    public int CityId { get; set; }
    public required string Address { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public City City { get; set; } = null!;
    public ICollection<Listing> Listings { get; set; } = new List<Listing>();
}

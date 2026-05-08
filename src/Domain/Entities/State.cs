using Heven.Api.Domain.Common;

namespace Heven.Api.Domain.Entities;

public class State : BaseEntity
{
    public int CountryId { get; set; }
    public required string Name { get; set; }

    public Country Country { get; set; } = null!;
    public ICollection<City> Cities { get; set; } = new List<City>();
}

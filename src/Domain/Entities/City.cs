using Heven.Api.Domain.Common;

namespace Heven.Api.Domain.Entities;

public class City : BaseEntity
{
    public int StateId { get; set; }
    public required string Name { get; set; }

    public State State { get; set; } = null!;
    public ICollection<Location> Locations { get; set; } = new List<Location>();
}

using Heven.Api.Domain.Common;

namespace Heven.Api.Domain.Entities;

public class Country : BaseEntity
{
    public required string Name { get; set; }
    public required string Code { get; set; } // ISO 3166-1 alpha-2, e.g. "VN", "US"

    public ICollection<State> States { get; set; } = new List<State>();
}

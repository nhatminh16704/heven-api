using Heven.Api.Domain.Common;

namespace Heven.Api.Domain.Entities;

public class Category : BaseEntity
{
    public required string Name { get; set; }
    public string? Icon { get; set; }
    public int SortOrder { get; set; }

    public ICollection<Listing> Listings { get; set; } = new List<Listing>();
}

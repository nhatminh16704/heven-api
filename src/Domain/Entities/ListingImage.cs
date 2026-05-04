using Heven.Api.Domain.Common;

namespace Heven.Api.Domain.Entities;

public class ListingImage : BaseEntity
{
    public int ListingId { get; set; }
    public required string Url { get; set; }
    public bool IsPrimary { get; set; }
    public int SortOrder { get; set; }

    public Listing Listing { get; set; } = null!;
}

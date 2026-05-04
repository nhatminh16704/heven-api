using Heven.Api.Domain.Common;

namespace Heven.Api.Domain.Entities;

public class Conversation : BaseAuditableEntity
{
    public int ListingId { get; set; }
    public required string GuestId { get; set; }
    public required string HostId { get; set; }

    public DateTimeOffset? LastMessageAt { get; set; }

    public Listing Listing { get; set; } = null!;
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}

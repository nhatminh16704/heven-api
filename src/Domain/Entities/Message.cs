using Heven.Api.Domain.Common;

namespace Heven.Api.Domain.Entities;

public class Message : BaseEntity
{
    public int ConversationId { get; set; }
    public required string SenderId { get; set; }

    public required string Content { get; set; }
    public bool IsRead { get; set; } = false;

    public DateTimeOffset? ReadAt { get; set; }
    public DateTimeOffset SentAt { get; set; } = DateTimeOffset.UtcNow;

    public Conversation Conversation { get; set; } = null!;
}

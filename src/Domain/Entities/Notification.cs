using Heven.Api.Domain.Common;
using Heven.Api.Domain.Enums;

namespace Heven.Api.Domain.Entities;

public class Notification : BaseAuditableEntity
{
    public required string UserId { get; set; }

    public NotificationType Type { get; set; }
    public required string Title { get; set; }
    public required string Body { get; set; }
    public string? Data { get; set; } 

    public bool IsRead { get; set; } = false;
    public DateTimeOffset? ReadAt { get; set; }
}

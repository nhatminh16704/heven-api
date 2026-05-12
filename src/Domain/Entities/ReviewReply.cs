namespace Heven.Api.Domain.Entities;

public class ReviewReply : BaseAuditableEntity
{
    public int ReviewId { get; set; }
    public required string AuthorId { get; set; }
    public required string Comment { get; set; }

    public Review Review { get; set; } = null!;
    public UserProfile AuthorProfile { get; set; } = null!;
}

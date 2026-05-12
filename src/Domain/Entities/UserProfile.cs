using Heven.Api.Domain.Common;

namespace Heven.Api.Domain.Entities;

public class UserProfile : BaseAuditableEntity
{
    public required string UserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
    public int? CityId { get; set; }

    public City? City { get; set; }
}

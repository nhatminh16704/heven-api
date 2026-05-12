using Microsoft.AspNetCore.Identity;

namespace Heven.Api.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public bool IdentityVerified { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; set; }
}

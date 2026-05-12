using Heven.Api.Infrastructure.Identity;

namespace Heven.Api.Web.Endpoints;

public class Users : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPut(UpdateProfile, "profile")
            .MapIdentityApi<ApplicationUser>();
    }

    public async Task<IResult> UpdateProfile(ISender sender, Heven.Api.Application.Users.Commands.UpdateProfile.UpdateProfileCommand command)
    {
        await sender.Send(command);
        return Results.NoContent();
    }
}

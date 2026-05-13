using Heven.Api.Application.Bookings.Commands.CreateCheckoutSession;


namespace Heven.Api.Web.Endpoints;

public class Bookings : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(CreateCheckoutSession, "checkout");
    }

    public async Task<string> CreateCheckoutSession(ISender sender, CreateCheckoutSessionCommand command)
    {
        return await sender.Send(command);
    }
}

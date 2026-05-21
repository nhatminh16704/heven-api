using Heven.Api.Application.Bookings.Commands.CreateBooking;
using Heven.Api.Application.Bookings.Commands.CreateCheckoutSession;
using Heven.Api.Application.Bookings.Commands.CancelBooking;


namespace Heven.Api.Web.Endpoints;

public class Bookings : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(CreateBooking)
            .MapPost(CreateCheckoutSession, "checkout")
            .MapPost(CancelBooking, "{id}/cancel");
    }

    public async Task<IResult> CreateBooking(ISender sender, CreateBookingCommand command)
    {
        var id = await sender.Send(command);
        return Results.Ok(id);
    }

    public async Task<IResult> CreateCheckoutSession(ISender sender, CreateCheckoutSessionCommand command)
    {
        var result = await sender.Send(command);
        return Results.Ok(result);
    }

    public async Task<IResult> CancelBooking(ISender sender, int id)
    {
        await sender.Send(new CancelBookingCommand(id));
        return Results.NoContent();
    }
}

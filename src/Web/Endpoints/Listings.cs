using Heven.Api.Application.Listings.Commands.CreateListing;
using Heven.Api.Application.Listings.Queries.GetListingById;
using Heven.Api.Web.Infrastructure;
using MediatR;

namespace Heven.Api.Web.Endpoints;

public class Listings : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(GetListingById, "{id}")
            .MapPost(CreateListing);
    }

    public async Task<ListingDto> GetListingById(ISender sender, int id)
    {
        return await sender.Send(new GetListingByIdQuery { Id = id });
    }

    public async Task<IResult> CreateListing(ISender sender, CreateListingCommand command)
    {
        var id = await sender.Send(command);
        return Results.CreatedAtRoute(nameof(GetListingById), new { id }, id);
    }
}

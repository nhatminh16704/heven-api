using Heven.Api.Application.Listings.Queries.GetListingById;
using Heven.Api.Web.Infrastructure;
using MediatR;

namespace Heven.Api.Web.Endpoints;

public class Listings : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(GetListingById, "{id}");
    }

    public async Task<ListingDto> GetListingById(ISender sender, int id)
    {
        return await sender.Send(new GetListingByIdQuery { Id = id });
    }
}

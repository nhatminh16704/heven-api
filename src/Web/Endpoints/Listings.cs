using Heven.Api.Application.Common.Models;
using Heven.Api.Application.Listings.Commands.CreateListing;
using Heven.Api.Application.Listings.Commands.CreateReview;
using Heven.Api.Application.Listings.Commands.UpdateListing;
using Heven.Api.Application.Listings.Commands.UpdateReview; // Nhớ Require using
using Heven.Api.Application.Listings.Queries.GetListingById;
using Heven.Api.Application.Listings.Queries.GetListingsWithPagination;
using Heven.Api.Application.Listings.Queries.GetListingReviewsWithPagination;
using Heven.Api.Web.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Heven.Api.Web.Endpoints;

public class Listings : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(GetListingsWithPagination)
            .MapGet(GetListingById, "{id}")
            .MapGet(GetListingReviewsWithPagination, "{listingId}/reviews")
            .MapPost(CreateListing)
            .MapPost(CreateListingReview, "{listingId}/reviews")
            .MapPut(UpdateListingReview, "{listingId}/reviews/{id}")
            .MapPut(UpdateListing, "{id}");
    }

    public async Task<PaginatedList<ListingBriefDto>> GetListingsWithPagination(ISender sender, [AsParameters] GetListingsWithPaginationQuery query)
    {
        return await sender.Send(query);
    }

    public async Task<ListingDto> GetListingById(ISender sender, int id)
    {
        return await sender.Send(new GetListingByIdQuery { Id = id });
    }

    public async Task<PaginatedList<ReviewDto>> GetListingReviewsWithPagination(ISender sender, [AsParameters] GetListingReviewsWithPaginationQuery query)
    {
        return await sender.Send(query);
    }

    public async Task<IResult> CreateListing(ISender sender, CreateListingCommand command)
    {
        var id = await sender.Send(command);
        return Results.CreatedAtRoute(nameof(GetListingById), new { id }, id);
    }

    public async Task<IResult> CreateListingReview(ISender sender, int listingId, CreateReviewCommand command)
    {
        if (listingId != command.ListingId) return Results.BadRequest();
        var id = await sender.Send(command);
        return Results.Ok(id);
    }

    public async Task<IResult> UpdateListingReview(ISender sender, int listingId, int id, UpdateReviewCommand command)
    {
        if (listingId != command.ListingId || id != command.Id) return Results.BadRequest();
        await sender.Send(command);
        return Results.NoContent();
    }

    public async Task<IResult> UpdateListing(ISender sender, int id, UpdateListingCommand command)
    {
        if (id != command.Id) return Results.BadRequest();
        await sender.Send(command);
        return Results.NoContent();
    }
}

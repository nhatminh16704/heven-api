using Heven.Api.Application.Common.Exceptions;
using Heven.Api.Application.Common.Interfaces;
using Heven.Api.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Heven.Api.Application.Listings.Queries.GetListingById;

public record GetListingByIdQuery : IRequest<ListingDto>
{
    public int Id { get; init; }
}

public class GetListingByIdQueryHandler : IRequestHandler<GetListingByIdQuery, ListingDto>
{
    private readonly IApplicationDbContext _context;

    public GetListingByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ListingDto> Handle(GetListingByIdQuery request, CancellationToken cancellationToken)
    {
        var listing = await _context.Listings
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new ListingDto
            {
                Id = x.Id,
                HostId = x.HostId,
                Title = x.Title,
                Description = x.Description,
                PricePerNight = x.PricePerNight,
                MaxGuests = x.MaxGuests,
                Bedrooms = x.Bedrooms,
                Status = x.Status.ToString()
            })
            .FirstOrDefaultAsync(cancellationToken);

        // Ném exception nếu không tìm thấy, Middleware sẽ lo việc trả về lỗi 404 cho Client
        if (listing == null)
        {
            throw new NotFoundException(nameof(Listing), request.Id.ToString());
        }

        return listing;
    }
}

using AutoMapper;
using AutoMapper.QueryableExtensions;
using Heven.Api.Application.Common.Caching;
using Heven.Api.Application.Common.Caching.Keys;
using Heven.Api.Application.Common.Exceptions;
using Heven.Api.Application.Common.Interfaces;
using Heven.Api.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Heven.Api.Application.Listings.Queries.GetListingById;

public record GetListingByIdQuery : IRequest<ListingDto>, ICacheable
{
    public int Id { get; init; }
    
    public string CacheKey => ListingCacheKeys.Details(Id);
    public TimeSpan? Expiration => TimeSpan.FromMinutes(15);
}

public class GetListingByIdQueryHandler : IRequestHandler<GetListingByIdQuery, ListingDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetListingByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ListingDto> Handle(GetListingByIdQuery request, CancellationToken cancellationToken)
    {
        var listing = await _context.Listings
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .ProjectTo<ListingDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (listing == null)
        {
            throw new NotFoundException(nameof(Listing), request.Id.ToString());
        }

        return listing;
    }
}

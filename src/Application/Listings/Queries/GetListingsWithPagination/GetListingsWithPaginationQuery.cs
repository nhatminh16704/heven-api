using AutoMapper;
using AutoMapper.QueryableExtensions;
using Heven.Api.Application.Common.Caching;
using Heven.Api.Application.Common.Caching.Keys;
using Heven.Api.Application.Common.Interfaces;
using Heven.Api.Application.Common.Mappings;
using Heven.Api.Application.Common.Models;
using MediatR;

namespace Heven.Api.Application.Listings.Queries.GetListingsWithPagination;

public record GetListingsWithPaginationQuery : IRequest<PaginatedList<ListingBriefDto>>, ICacheable
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;

    public string CacheKey => ListingCacheKeys.List(PageNumber, PageSize);
    public TimeSpan? Expiration => TimeSpan.FromMinutes(15);
}

public class GetListingsWithPaginationQueryHandler : IRequestHandler<GetListingsWithPaginationQuery, PaginatedList<ListingBriefDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetListingsWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<ListingBriefDto>> Handle(GetListingsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        return await _context.Listings
            .OrderBy(x => x.Id)
            .ProjectTo<ListingBriefDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}

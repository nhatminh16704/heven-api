using AutoMapper;
using AutoMapper.QueryableExtensions;
using Heven.Api.Application.Common.Caching;
using Heven.Api.Application.Common.Caching.Keys;
using Heven.Api.Application.Common.Interfaces;
using Heven.Api.Application.Common.Mappings;
using Heven.Api.Application.Common.Models;
using Heven.Api.Domain.Entities;
using Heven.Api.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Heven.Api.Application.Listings.Queries.GetListingReviewsWithPagination;

public record GetListingReviewsWithPaginationQuery : IRequest<PaginatedList<ReviewDto>>, ICacheable
{
    public int ListingId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    
    // Implement caching logic
    public string CacheKey => ListingCacheKeys.Reviews(ListingId, PageNumber, PageSize);
    public TimeSpan? Expiration => TimeSpan.FromMinutes(15);
}

public class GetListingReviewsWithPaginationQueryHandler : IRequestHandler<GetListingReviewsWithPaginationQuery, PaginatedList<ReviewDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetListingReviewsWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<ReviewDto>> Handle(GetListingReviewsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        return await _context.Reviews
            .Where(r => r.ListingId == request.ListingId && r.Booking.Status == BookingStatus.Completed)
            .OrderByDescending(r => r.Created)
            .ProjectTo<ReviewDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}



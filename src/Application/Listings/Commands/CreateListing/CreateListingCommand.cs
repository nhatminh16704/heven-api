using AutoMapper;
using Heven.Api.Application.Common.Interfaces;
using Heven.Api.Domain.Entities;
using MediatR;

namespace Heven.Api.Application.Listings.Commands.CreateListing;

public record CreateListingCommand : IRequest<int>
{
    public required string HostId { get; init; }
    public int CategoryId { get; init; }
    public int LocationId { get; init; }

    public required string Title { get; init; }
    public string? Description { get; init; }
    public decimal PricePerNight { get; init; }
    public decimal CleaningFee { get; init; }
    public int MaxGuests { get; init; }
    public int Bedrooms { get; init; }
    public int Beds { get; init; }
    public int Bathrooms { get; init; }
    public string? PropertyType { get; init; }
    public bool InstantBook { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<CreateListingCommand, Listing>();
        }
    }
}

public class CreateListingCommandHandler : IRequestHandler<CreateListingCommand, int>
{ 
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateListingCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<int> Handle(CreateListingCommand request, CancellationToken cancellationToken)
    {
        var listing = _mapper.Map<Listing>(request);

        _context.Listings.Add(listing);
        await _context.SaveChangesAsync(cancellationToken);

        return listing.Id;
    }
}

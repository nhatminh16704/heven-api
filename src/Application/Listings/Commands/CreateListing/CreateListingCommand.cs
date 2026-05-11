using AutoMapper;
using Heven.Api.Application.Common.Interfaces;
using Heven.Api.Application.Common.Security;
using Heven.Api.Domain.Entities;
using MediatR;

namespace Heven.Api.Application.Listings.Commands.CreateListing;

[Authorize(Roles = "Host,Administrator")]
public record CreateListingCommand : IRequest<int>
{
    public int CategoryId { get; init; }

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

    public required CreateListingLocationDto Location { get; init; }
    public IReadOnlyCollection<int> AmenityIds { get; init; } = Array.Empty<int>();
    public IReadOnlyCollection<CreateListingImageDto> Images { get; init; } = Array.Empty<CreateListingImageDto>();

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<CreateListingCommand, Listing>()
                .ForMember(d => d.HostId, opt => opt.Ignore())
                .ForMember(d => d.Amenities, opt => opt.MapFrom(s => s. AmenityIds.Select(id => new ListingAmenity { AmenityId = id })));
        }
    }
}

public class CreateListingLocationDto
{
    public required string Address { get; init; }
    public int CityId { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<CreateListingLocationDto, Location>();
        }
    }
}

public class CreateListingImageDto
{
    public required string Url { get; init; }
    public bool IsPrimary { get; init; }
    public int SortOrder { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<CreateListingImageDto, ListingImage>();
        }
    }
}

public class CreateListingCommandHandler : IRequestHandler<CreateListingCommand, int>
{ 
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IUser _user;

    public CreateListingCommandHandler(IApplicationDbContext context, IMapper mapper, IUser user)
    {
        _context = context;
        _mapper = mapper;
        _user = user;
    }

    public async Task<int> Handle(CreateListingCommand request, CancellationToken cancellationToken)
    {
        var listing = _mapper.Map<Listing>(request);
        listing.HostId = _user.Id!;

        _context.Listings.Add(listing);
        await _context.SaveChangesAsync(cancellationToken);

        return listing.Id;
    }
}

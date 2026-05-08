using AutoMapper;
using Heven.Api.Domain.Entities;

namespace Heven.Api.Application.Listings.Queries.GetListingById;

public class ListingDto
{
    public int Id { get; init; }
    public string? HostId { get; init; }
    
    // Core details
    public string? Title { get; init; }
    public string? Description { get; init; }
    public decimal PricePerNight { get; init; }
    public decimal CleaningFee { get; init; }
    
    // Capacity
    public int MaxGuests { get; init; }
    public int Bedrooms { get; init; }
    public int Beds { get; init; }
    public int Bathrooms { get; init; }
    
    // Configs
    public string? PropertyType { get; init; }
    public bool InstantBook { get; init; }
    public string? Status { get; init; }
    
    // Stats
    public double RatingAverage { get; init; }
    public int ReviewCount { get; init; }

    // Related details
    public string? Category { get; init; }
    public ListingLocationDto? Location { get; init; }

    public IReadOnlyCollection<string> Amenities { get; init; } = Array.Empty<string>();
    public IReadOnlyCollection<ListingImageDto> Images { get; init; } = Array.Empty<ListingImageDto>();

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Listing, ListingDto>()
                .ForMember(d => d.Category, opt => opt.MapFrom(s => s.Category.Name))
                .ForMember(d => d.Amenities, opt => opt.MapFrom(s => s.Amenities.Select(a => a.Amenity.Name)));
        }
    }
}

public class ListingLocationDto
{
    public string? Address { get; init; }
    public string? City { get; init; }
    public string? State { get; init; }
    public string? Country { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Location, ListingLocationDto>()
                .ForMember(d => d.City, opt => opt.MapFrom(s => s.City.Name))
                .ForMember(d => d.State, opt => opt.MapFrom(s => s.City.State.Name))
                .ForMember(d => d.Country, opt => opt.MapFrom(s => s.City.State.Country.Name));
        }
    }
}

public class ListingImageDto
{
    public string? Url { get; init; }
    public bool IsPrimary { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<ListingImage, ListingImageDto>();
        }
    }
}

using AutoMapper;
using Heven.Api.Domain.Entities;

namespace Heven.Api.Application.Listings.Queries.GetListingsWithPagination;

public class ListingBriefDto
{
    public int Id { get; init; }
    public string? Title { get; init; }
    public string? PropertyType { get; init; }
    public string? City { get; init; }
    public string? Country { get; init; }
    public decimal PricePerNight { get; init; }
    public double RatingAverage { get; init; }
    public int ReviewCount { get; init; }
    public string? ThumbnailUrl { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Listing, ListingBriefDto>()
                .ForMember(d => d.City, opt => opt.MapFrom(s => s.Location.City.Name))
                .ForMember(d => d.Country, opt => opt.MapFrom(s => s.Location.City.State.Country.Name))
                .ForMember(d => d.ThumbnailUrl, opt => opt.MapFrom(s => 
                    s.Images.Where(i => i.IsPrimary).Select(i => i.Url).FirstOrDefault() ?? 
                    s.Images.Select(i => i.Url).FirstOrDefault()));
        }
    }
}

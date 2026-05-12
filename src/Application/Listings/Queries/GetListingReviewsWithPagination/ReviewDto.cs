using Heven.Api.Domain.Entities;
using AutoMapper;

namespace Heven.Api.Application.Listings.Queries.GetListingReviewsWithPagination;

public class ReviewDto
{
    public int Id { get; init; }
    public int Rating { get; init; }
    public string? Comment { get; init; }
    public DateTimeOffset Created { get; init; }
    public DateOnly CheckIn { get; init; }
    public DateOnly CheckOut { get; init; }
    public ReviewerDto Reviewer { get; init; } = null!;

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Review, ReviewDto>()
                .ForMember(d => d.Rating, opt => opt.MapFrom(s => s.OverallRating))
                .ForMember(d => d.CheckIn, opt => opt.MapFrom(s => s.Booking.CheckIn))
                .ForMember(d => d.CheckOut, opt => opt.MapFrom(s => s.Booking.CheckOut))
                .ForMember(d => d.Reviewer, opt => opt.MapFrom(s => s.AuthorProfile));
        }
    }
}

public class ReviewerDto
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public string? AvatarUrl { get; init; }
    public string? Location { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<UserProfile, ReviewerDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.UserId))
                .ForMember(d => d.Name, opt => opt.MapFrom(s => (s.FirstName + " " + s.LastName).Trim()))
                .ForMember(d => d.Location, opt => opt.MapFrom(s => 
                    s.City != null ? s.City.Name + (s.City.State != null ? ", " + s.City.State.Name : "") : null));
        }
    }
}
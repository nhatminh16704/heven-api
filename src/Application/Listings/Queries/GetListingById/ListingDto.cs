namespace Heven.Api.Application.Listings.Queries.GetListingById;

public class ListingDto
{
    public int Id { get; init; }
    public string? HostId { get; init; }
    public string? Title { get; init; }
    public string? Description { get; init; }
    public decimal PricePerNight { get; init; }
    public int MaxGuests { get; init; }
    public int Bedrooms { get; init; }
    public string? Status { get; init; }
}

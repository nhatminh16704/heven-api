using Heven.Api.Domain.Entities;

namespace Heven.Api.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<TodoList> TodoLists { get; }
    DbSet<TodoItem> TodoItems { get; }
    
    DbSet<Category> Categories { get; }
    DbSet<Country> Countries { get; }
    DbSet<State> States { get; }
    DbSet<City> Cities { get; }
    DbSet<Location> Locations { get; }
    DbSet<Amenity> Amenities { get; }
    DbSet<Listing> Listings { get; }
    DbSet<ListingImage> ListingImages { get; }
    DbSet<ListingAmenity> ListingAmenities { get; }
    DbSet<Booking> Bookings { get; }
    DbSet<Payment> Payments { get; }
    DbSet<BookingCancellation> BookingCancellations { get; }
    DbSet<Review> Reviews { get; }
    DbSet<Conversation> Conversations { get; }
    DbSet<Message> Messages { get; }
    DbSet<Notification> Notifications { get; }
    DbSet<UserProfile> UserProfiles { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

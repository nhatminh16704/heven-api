using Heven.Api.Application.Common.Interfaces;
using Heven.Api.Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Heven.Api.Application.Listings.EventHandlers;

public class ReviewCreatedEventHandler : INotificationHandler<ReviewCreatedEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<ReviewCreatedEventHandler> _logger;

    public ReviewCreatedEventHandler(IApplicationDbContext context, ILogger<ReviewCreatedEventHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(ReviewCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event Started: {DomainEvent} for Review ID {ReviewId}", 
            notification.GetType().Name, notification.Item.Id);

        var listingId = notification.Item.ListingId;

        // EF Core FindAsync sẽ tìm trên Database NẾU chưac có trong RAM
        var listing = await _context.Listings.FindAsync([listingId], cancellationToken);

        if (listing != null)
        {
            var currentCount = listing.ReviewCount;
            var currentTotalScore = listing.RatingAverage * currentCount;
            
            var newCount = currentCount + 1;
            var newAverage = (currentTotalScore + notification.Item.OverallRating) / newCount;

            listing.ReviewCount = newCount;
            listing.RatingAverage = Math.Round(newAverage, 2);

            _logger.LogInformation("Listing {ListingId} stats updated in ChangeTracker: Count={Count}, Rating={Rating}", 
                listing.Id, listing.ReviewCount, listing.RatingAverage);
        }
    }
}

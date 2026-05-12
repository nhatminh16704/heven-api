using Heven.Api.Application.Common.Interfaces;
using Heven.Api.Domain.Entities;
using MediatR;

namespace Heven.Api.Application.Listings.Commands.CreateReview;

public record CreateReviewCommand : IRequest<int>
{
    public int ListingId { get; init; }
    public int BookingId { get; init; }
    public int Rating { get; init; }
    public string? Comment { get; init; }
}

public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public CreateReviewCommandHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<int> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        var review = new Review
        {
            ListingId = request.ListingId,
            BookingId = request.BookingId,
            AuthorId = _user.Id!,
            OverallRating = request.Rating,
            Comment = request.Comment
        };

        _context.Reviews.Add(review);

        await _context.SaveChangesAsync(cancellationToken);

        return review.Id;
    }
}

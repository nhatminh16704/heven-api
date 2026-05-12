using Heven.Api.Application.Common.Exceptions;
using Heven.Api.Application.Common.Interfaces;
using Heven.Api.Domain.Entities;
using MediatR;

namespace Heven.Api.Application.Listings.Commands.UpdateReview;

public record UpdateReviewCommand : IRequest
{
    public int ListingId { get; init; }
    public int Id { get; init; }
    public int Rating { get; init; }
    public string? Comment { get; init; }
}

public class UpdateReviewCommandHandler : IRequestHandler<UpdateReviewCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public UpdateReviewCommandHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _context.Reviews
            .FindAsync([request.Id], cancellationToken);

        if (review == null || review.ListingId != request.ListingId)
        {
            throw new NotFoundException(nameof(Review), request.Id.ToString());
        }

        if (review.AuthorId != _user.Id)
        {
            throw new ForbiddenAccessException();
        }

        review.OverallRating = request.Rating;
        review.Comment = request.Comment;

        await _context.SaveChangesAsync(cancellationToken);
    }
}

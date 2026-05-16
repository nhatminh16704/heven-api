using Heven.Api.Application.Common.Exceptions;
using Heven.Api.Application.Common.Interfaces;
using Heven.Api.Domain.Constants;
using Heven.Api.Domain.Entities;
using Heven.Api.Domain.Enums;
using Heven.Api.Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ConflictException = Heven.Api.Application.Common.Exceptions.ConflictException;
using ForbiddenAccessException = Heven.Api.Application.Common.Exceptions.ForbiddenAccessException;

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
        if (string.IsNullOrEmpty(_user.Id))
        {
            throw new UnauthorizedAccessException();
        }

        var hasProfile = await _context.UserProfiles.AnyAsync(p => p.UserId == _user.Id, cancellationToken);
        if (!hasProfile)
        {
             throw new ForbiddenAccessException("You need to complete your profile before writing a review.", ErrorCodes.User.ProfileIncomplete);
        }

        var bookingCompleted = await _context.Bookings.AnyAsync(b =>
            b.Id == request.BookingId &&
            b.ListingId == request.ListingId &&
            b.GuestId == _user.Id &&
            b.Status == BookingStatus.Completed, cancellationToken);

        if (!bookingCompleted)
        {
            throw new ConflictException("You cannot review this listing because the booking information does not match or the booking is not completed.", ErrorCodes.Review.BookingNotCompleted);
        }

        var alreadyReviewed = await _context.Reviews.AnyAsync(r =>
            r.BookingId == request.BookingId &&
            r.AuthorId == _user.Id, cancellationToken);

        if (alreadyReviewed)
        {
            throw new ConflictException("You have already reviewed this booking.", ErrorCodes.Review.AlreadyReviewed);
        }

        var review = new Review
        {
            ListingId = request.ListingId,
            BookingId = request.BookingId,
            AuthorId = _user.Id!,
            OverallRating = request.Rating,
            Comment = request.Comment
        };

        // Gắn Event thông báo có 1 Review mới được tạo!
        review.AddDomainEvent(new ReviewCreatedEvent(review));

        _context.Reviews.Add(review);

        await _context.SaveChangesAsync(cancellationToken);

        return review.Id;
    }
}

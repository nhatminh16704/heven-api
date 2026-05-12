using FluentValidation;
using Heven.Api.Application.Common.Interfaces;
using Heven.Api.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Heven.Api.Application.Listings.Commands.CreateReview;

public class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public CreateReviewCommandValidator(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;

        RuleFor(v => v.ListingId).GreaterThan(0);
        RuleFor(v => v.BookingId).GreaterThan(0);

        RuleFor(v => v.Rating)
            .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5 stars.");

        RuleFor(v => v.Comment)
            .MaximumLength(2000).WithMessage("Comment must not exceed 2000 characters.");

        // Thêm Cascade(CascadeMode.Stop) tại đây để ngắt vòng lặp xác thực nếu một quy tắc bị lỗi.
        RuleFor(v => v)
            .Cascade(CascadeMode.Stop)
            .MustAsync(HaveProfileAsync).WithMessage("You need to complete your profile before writing a review.")
            .MustAsync(HaveCompletedBookingAsync).WithMessage("You cannot review this listing because the booking information does not match or the booking is not completed.")
            .MustAsync(HaveNotReviewedYetAsync).WithMessage("You have already reviewed this booking.");
    }

    private async Task<bool> HaveProfileAsync(CreateReviewCommand command, CancellationToken cancellationToken)
    {
        if (_user.Id == null) return false;
        return await _context.UserProfiles.AnyAsync(p => p.UserId == _user.Id, cancellationToken);
    }

    private async Task<bool> HaveCompletedBookingAsync(CreateReviewCommand command, CancellationToken cancellationToken)
    {
        if (_user.Id == null) return false;
        return await _context.Bookings.AnyAsync(b =>
            b.Id == command.BookingId &&
            b.ListingId == command.ListingId &&
            b.GuestId == _user.Id &&
            b.Status == BookingStatus.Completed, cancellationToken);
    }

    private async Task<bool> HaveNotReviewedYetAsync(CreateReviewCommand command, CancellationToken cancellationToken)
    {
        if (_user.Id == null) return false;
        return !await _context.Reviews.AnyAsync(r =>
            r.BookingId == command.BookingId &&
            r.AuthorId == _user.Id, cancellationToken);
    }
}

using FluentValidation;

namespace Heven.Api.Application.Listings.Commands.CreateReview;

public class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewCommandValidator()
    {
        RuleFor(v => v.ListingId).GreaterThan(0);
        RuleFor(v => v.BookingId).GreaterThan(0);
        RuleFor(v => v.Rating).InclusiveBetween(1, 5);
        RuleFor(v => v.Comment).MaximumLength(2000);
    }
}

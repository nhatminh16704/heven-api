using FluentValidation;

namespace Heven.Api.Application.Listings.Commands.UpdateReview;

public class UpdateReviewCommandValidator : AbstractValidator<UpdateReviewCommand>
{
    public UpdateReviewCommandValidator()
    {
        RuleFor(v => v.ListingId).GreaterThan(0);
        RuleFor(v => v.Id).GreaterThan(0);

        RuleFor(v => v.Rating)
            .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5 stars.");

        RuleFor(v => v.Comment)
            .MaximumLength(2000).WithMessage("Comment must not exceed 2000 characters.");
    }
}

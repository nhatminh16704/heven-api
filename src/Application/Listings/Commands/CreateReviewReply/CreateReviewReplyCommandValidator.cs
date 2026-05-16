using FluentValidation;

namespace Heven.Api.Application.Listings.Commands.CreateReviewReply;

public class CreateReviewReplyCommandValidator : AbstractValidator<CreateReviewReplyCommand>
{
    public CreateReviewReplyCommandValidator()
    {
        RuleFor(v => v.ListingId).GreaterThan(0);
        RuleFor(v => v.ReviewId).GreaterThan(0);

        RuleFor(v => v.Comment)
            .NotEmpty().WithMessage("Comment is required.")
            .MaximumLength(2000).WithMessage("Comment must not exceed 2000 characters.");
    }
}

using FluentValidation;
using Heven.Api.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Heven.Api.Application.Listings.Commands.CreateReviewReply;

public class CreateReviewReplyCommandValidator : AbstractValidator<CreateReviewReplyCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateReviewReplyCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.ListingId).GreaterThan(0);
        RuleFor(v => v.ReviewId).GreaterThan(0);

        RuleFor(v => v.Comment)
            .NotEmpty().WithMessage("Comment is required.")
            .MaximumLength(2000).WithMessage("Comment must not exceed 2000 characters.");

        RuleFor(v => v)
            .Cascade(CascadeMode.Stop)
            .MustAsync(HaveNotRepliedYetAsync).WithMessage("You have already replied to this review.");
    }

    private async Task<bool> HaveNotRepliedYetAsync(CreateReviewReplyCommand command, CancellationToken cancellationToken)
    {
        return !await _context.ReviewReplies.AnyAsync(r => r.ReviewId == command.ReviewId, cancellationToken);
    }
}

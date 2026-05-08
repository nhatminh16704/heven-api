using FluentValidation;

namespace Heven.Api.Application.Listings.Commands.CreateListing;

public class CreateListingCommandValidator : AbstractValidator<CreateListingCommand>
{
    public CreateListingCommandValidator()
    {
        RuleFor(x => x.HostId)
            .NotEmpty();

        RuleFor(x => x.CategoryId)
            .GreaterThan(0);

        RuleFor(x => x.LocationId)
            .GreaterThan(0);

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.PricePerNight)
            .GreaterThan(0);

        RuleFor(x => x.MaxGuests)
            .GreaterThan(0);

        RuleFor(x => x.Bedrooms)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Beds)
            .GreaterThan(0);

        RuleFor(x => x.Bathrooms)
            .GreaterThan(0);
    }
}

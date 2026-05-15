using FluentValidation;

namespace Heven.Api.Application.Listings.Commands.UpdateListing;

public class UpdateListingCommandValidator : AbstractValidator<UpdateListingCommand>
{
    public UpdateListingCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Listing ID must be greater than 0.");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0);

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.PricePerNight)
            .GreaterThan(0).WithMessage("Price per night must be greater than 0.");

        RuleFor(x => x.MaxGuests)
            .GreaterThan(0);

        RuleFor(x => x.Bedrooms)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Beds)
            .GreaterThan(0);

        RuleFor(x => x.Bathrooms)
            .GreaterThan(0);

        RuleFor(x => x.Location)
            .NotNull()
            .SetValidator(new UpdateListingLocationDtoValidator());
    }
}

public class UpdateListingLocationDtoValidator : AbstractValidator<UpdateListingLocationDto>
{
    public UpdateListingLocationDtoValidator()
    {
        RuleFor(x => x.Address)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.CityId)
            .GreaterThan(0);

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90);

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180);
    }
}

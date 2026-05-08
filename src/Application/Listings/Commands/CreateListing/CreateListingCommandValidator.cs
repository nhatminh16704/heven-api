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

        RuleFor(x => x.Location)
            .NotNull()
            .SetValidator(new CreateListingLocationDtoValidator());
            
        RuleForEach(x => x.Images)
            .SetValidator(new CreateListingImageDtoValidator());
    }
}

public class CreateListingLocationDtoValidator : AbstractValidator<CreateListingLocationDto>
{
    public CreateListingLocationDtoValidator()
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

public class CreateListingImageDtoValidator : AbstractValidator<CreateListingImageDto>
{
    public CreateListingImageDtoValidator()
    {
        RuleFor(x => x.Url)
            .NotEmpty();
    }
}

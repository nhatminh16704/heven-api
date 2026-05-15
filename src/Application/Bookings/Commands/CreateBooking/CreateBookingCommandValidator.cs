using FluentValidation;
using Heven.Api.Application.Common.Interfaces;
using Heven.Api.Domain.Constants;
using Heven.Api.Domain.Enums;

namespace Heven.Api.Application.Bookings.Commands.CreateBooking;

public class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;
    private readonly IIdentityService _identityService;

    public CreateBookingCommandValidator(IApplicationDbContext context, IUser user, IIdentityService identityService)
    {
        _context = context;
        _user = user;
        _identityService = identityService;

        RuleFor(v => v.ListingId).GreaterThan(0);
        RuleFor(v => v.CheckOut).GreaterThan(v => v.CheckIn);
        RuleFor(v => v.GuestCount).InclusiveBetween(1, 50);
        RuleFor(v => v.SpecialRequests).MaximumLength(1000);

        RuleFor(v => v)
            .Cascade(CascadeMode.Stop)
            .MustAsync(HaveUserProfileAsync)
                .WithMessage("You need to complete your profile before booking.")
                .WithErrorCode(ErrorCodes.User.ProfileIncomplete)
            .MustAsync(HaveVerifiedEmailAsync)
                .WithMessage("Verify your email before booking.")
                .WithErrorCode(ErrorCodes.User.EmailNotVerified)
            .CustomAsync(ValidateListingAsync);
    }

    private async Task<bool> HaveUserProfileAsync(CreateBookingCommand command, CancellationToken cancellationToken)
    {
        return await _context.UserProfiles.AnyAsync(p => p.UserId == _user.Id!, cancellationToken);
    }

    private async Task<bool> HaveVerifiedEmailAsync(CreateBookingCommand command, CancellationToken cancellationToken)
    {
        return await _identityService.IsEmailConfirmedAsync(_user.Id!);
    }

    private async Task ValidateListingAsync(
        CreateBookingCommand command,
        ValidationContext<CreateBookingCommand> context,
        CancellationToken cancellationToken)
    {
        var listing = await _context.Listings.FirstOrDefaultAsync(l => l.Id == command.ListingId, cancellationToken);
        if (listing == null)
        {
            context.AddFailure(nameof(command.ListingId), "Listing was not found.");
            return;
        }

        if (listing.Status != ListingStatus.Active)
        {
            context.AddFailure(nameof(command.ListingId), "This listing is not available for booking.");
            return;
        }

        if (listing.HostId == _user.Id!)
        {
            context.AddFailure(nameof(command.ListingId), "You cannot book your own listing.");
            return;
        }

        if (command.GuestCount > listing.MaxGuests)
        {
            context.AddFailure(nameof(command.GuestCount), $"Guest count cannot exceed {listing.MaxGuests}.");
        }
    }
}

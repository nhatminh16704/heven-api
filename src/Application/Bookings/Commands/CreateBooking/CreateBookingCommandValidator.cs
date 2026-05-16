using FluentValidation;

namespace Heven.Api.Application.Bookings.Commands.CreateBooking;

public class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidator()
    {
        RuleFor(v => v.ListingId).GreaterThan(0);
        RuleFor(v => v.CheckOut).GreaterThan(v => v.CheckIn);
        RuleFor(v => v.GuestCount).InclusiveBetween(1, 50);
        RuleFor(v => v.SpecialRequests).MaximumLength(1000);
    }
}

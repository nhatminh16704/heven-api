using FluentValidation;

namespace Heven.Api.Application.Users.Commands.UpdateProfile;

public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(v => v.FirstName)
            .MaximumLength(100);

        RuleFor(v => v.LastName)
            .MaximumLength(100);

        RuleFor(v => v.AvatarUrl)
            .MaximumLength(500);

        RuleFor(v => v.Bio)
            .MaximumLength(1000);
    }
}

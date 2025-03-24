using FluentValidation;

namespace MinimalApis.Commands.UpdateProfile;

public class UpdateProfileCommandValidator
    : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(r => r.Name)
            .NotNull()
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(r => r.Email)
            .NotNull()
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(50);
    }
}

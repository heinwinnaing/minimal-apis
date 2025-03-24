using FluentValidation;

namespace MinimalApis.Commands.RegisterProfile;

public class RegisterProfileCommandValidator
    : AbstractValidator<RegisterProfileCommand>
{
    public RegisterProfileCommandValidator()
    {
        RuleFor(r => r.Name)
            .NotNull()
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(r => r.Phone)
            .NotNull()
            .NotEmpty()
            .MaximumLength(50)
            .Matches("^[0-9]");

        RuleFor(r => r.Email)
            .NotNull()
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(50);
    }
}
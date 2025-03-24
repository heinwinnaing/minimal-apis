using FluentValidation;

namespace MinimalApis.Commands.VerifyOtp;

public class VerifyOtpCommandValidator
    : AbstractValidator<VerifyOtpCommand>
{
    public VerifyOtpCommandValidator()
    {
        RuleFor(r => r.OtpCode)
            .NotNull()
            .NotEmpty()
            .Length(6);
    }
}
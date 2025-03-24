using FluentValidation;

namespace MinimalApis.Commands.RequestOtp;

public class RequestOtpCommandValidator
    : AbstractValidator<RequestOtpCommand>
{
    public RequestOtpCommandValidator()
    {
        RuleFor(r => r.Phone)
            .NotNull()
            .NotEmpty()
            .MaximumLength(50);
    }
}
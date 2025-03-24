using MediatR;
using MinimalApis.Model;

namespace MinimalApis.Commands.VerifyOtp;

public class VerifyOtpCommand
    : IRequest<ResultModel<VerifyOtpCommandDto>>
{
    public Guid Token { get; set; }
    public string OtpCode { get; set; } = null!;
}

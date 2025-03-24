using MediatR;
using MinimalApis.Model;
using System.ComponentModel;

namespace MinimalApis.Commands.RequestOtp;

public class RequestOtpCommand
    : IRequest<ResultModel<RequestOtpCommandDto>>
{
    [DefaultValue("123456789")]
    public string Phone { get; set; } = null!;
}

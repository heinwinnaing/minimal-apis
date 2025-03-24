using System.ComponentModel;

namespace MinimalApis.Commands.RequestOtp;

public class RequestOtpCommandDto
{
    public TimeSpan ExpiryIn {  get; set; }

    [DefaultValue("000000")]
    public string Token { get; set; } = null!;
}
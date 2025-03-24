using MinimalApis.Model;

namespace MinimalApis.Commands.VerifyOtp;

public class VerifyOtpCommandDto
{
    public JwtTokenModel AccessToken { get; set; } = null!;
    public ProfileDto Profile { get; set; } = null!;
}
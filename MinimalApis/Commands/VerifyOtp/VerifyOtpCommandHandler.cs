using MediatR;
using Microsoft.EntityFrameworkCore;
using MinimalApis.Domain;
using MinimalApis.Model;
using MinimalApis.Services;
using System.Security.Claims;

namespace MinimalApis.Commands.VerifyOtp;

public class VerifyOtpCommandHandler(
    VerifyOtpCommandValidator validator,
    JwtTokenService jwtTokenService,
    IDbContext dbContext)
    : IRequestHandler<VerifyOtpCommand, ResultModel<VerifyOtpCommandDto>>
{
    public async Task<ResultModel<VerifyOtpCommandDto>> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(request);
        if(!validationResult.IsValid)
        {
            var erros = validationResult.Errors.Select(s => s.ErrorMessage).ToArray();
            return ResultModel<VerifyOtpCommandDto>.Error(400, erros);
        }
        var account = await dbContext
            .Accounts
            .FirstOrDefaultAsync(r => r.Id == request.Token, cancellationToken);
        if(account is null
            || request.OtpCode != DateTime.Now.ToString("ddMMyy"))
        {
            return ResultModel<VerifyOtpCommandDto>.Error(400, "Invalid otp code or expired.");
        }

        Claim[] claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, $"{account.Id}"),
            new Claim(ClaimTypes.MobilePhone, $"{account.Phone}")
        };
        var accessToken = jwtTokenService.CreateAccessToken(claims, out DateTime expiryIn);
        var data = new VerifyOtpCommandDto
        {
            AccessToken = new JwtTokenModel
            {
                Token = accessToken,
                ExpiryIn = expiryIn
            },
            Profile = new ProfileDto
            {
                Id = account.Id,
                Name = account.Name,
                Email = account.Email,
                Phone = account.Phone,
            }
        };

        return ResultModel<VerifyOtpCommandDto>.Success(data);
    }
}

using MediatR;
using Microsoft.EntityFrameworkCore;
using MinimalApis.Domain;
using MinimalApis.Model;
using MinimalApis.Services;

namespace MinimalApis.Commands.Setup2Fa;

public class Setup2FaCommandHandler(
    ITwoFactorService twoFactorService,
    IDbContext dbContext)
    : IRequestHandler<Setup2FaCommand, ResultModel<Setup2FaCommandDto>>
{
    public async Task<ResultModel<Setup2FaCommandDto>> Handle(Setup2FaCommand request, CancellationToken cancellationToken)
    {
        var profile = await dbContext
            .Accounts
            .FirstOrDefaultAsync(r => r.Id == request.id, cancellationToken);
        if (profile is null)
            return ResultModel<Setup2FaCommandDto>.Error(400, "Unable to setup");

        var tfa = twoFactorService.Generate(profile.Email);
        await dbContext.TwoFactors.AddAsync(new Domain.Accounts.TwoFactor 
        {
            AccountId = profile.Id,
            SecretKey = tfa.secretKey
        });
        await dbContext.SaveChangesAsync(cancellationToken);

        return ResultModel<Setup2FaCommandDto>.Success(new Setup2FaCommandDto(tfa.secretKey, Convert.ToBase64String(tfa.qrImage)));
    }
}

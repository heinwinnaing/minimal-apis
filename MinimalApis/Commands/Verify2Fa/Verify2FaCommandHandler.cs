using MediatR;
using Microsoft.EntityFrameworkCore;
using MinimalApis.Domain;
using MinimalApis.Model;
using MinimalApis.Services;

namespace MinimalApis.Commands.Verify2Fa;

public class Verify2FaCommandHandler(
    ITwoFactorService twoFactorService,
    IDbContext dbContext) : IRequestHandler<Verify2FaCommand, ResultModel>
{
    public async Task<ResultModel> Handle(Verify2FaCommand request, CancellationToken cancellationToken)
    {
        var profile = await dbContext
            .Accounts
            .FirstOrDefaultAsync(r => r.Id == request.id, cancellationToken);
        if (profile is null)
            return ResultModel.Error(400, "Code invalid or expired");

        var my2fa = await dbContext
            .TwoFactors
            .FirstOrDefaultAsync(r => r.AccountId == request.id, cancellationToken);
        if (my2fa is null)
            return ResultModel.Error(400, "Code invalid or expired");

        if(twoFactorService.Verify(my2fa.SecretKey, request.code))
            return ResultModel.Success();

        return ResultModel.Error(400, "Code invalid or expired");
    }
}

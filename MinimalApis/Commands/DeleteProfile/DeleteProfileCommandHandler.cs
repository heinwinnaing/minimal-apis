using MediatR;
using Microsoft.EntityFrameworkCore;
using MinimalApis.Domain;
using MinimalApis.Model;

namespace MinimalApis.Commands.DeleteProfile;

public class DeleteProfileCommandHandler(
    IDbContext dbContext)
    : IRequestHandler<DeleteProfileCommand, ResultModel>
{
    public async Task<ResultModel> Handle(DeleteProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await dbContext
            .Accounts
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
        if (profile is null)
            return ResultModel.Error(400, "Unable to delete profile");

        dbContext
            .Accounts
            .Remove(profile);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ResultModel.Success();
    }
}

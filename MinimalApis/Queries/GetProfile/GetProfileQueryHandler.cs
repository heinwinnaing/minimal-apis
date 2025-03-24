using MediatR;
using Microsoft.EntityFrameworkCore;
using MinimalApis.Domain;
using MinimalApis.Model;

namespace MinimalApis.Queries.GetProfile;

public class GetProfileQueryHandler(
    IDbContext dbContext)
    : IRequestHandler<GetProfileQuery, ResultModel<GetProfileDto>>
{
    public async Task<ResultModel<GetProfileDto>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        var profile = await dbContext
            .Accounts
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
        if (profile is null)
            return ResultModel<GetProfileDto>.Error(400, "Unable to get profile");

        return ResultModel<GetProfileDto>.Success(new GetProfileDto
        {
            Id = profile.Id,
            Name = profile.Name,
            Phone = profile.Phone,
            Email = profile.Email
        });
    }
}

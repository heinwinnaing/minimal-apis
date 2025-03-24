using MediatR;
using Microsoft.EntityFrameworkCore;
using MinimalApis.Domain;
using MinimalApis.Model;

namespace MinimalApis.Commands.UpdateProfile;

public class UpdateProfileCommandHandler(
    UpdateProfileCommandValidator validator,
    IDbContext dbContext)
    : IRequestHandler<UpdateProfileCommand, ResultModel>
{
    public async Task<ResultModel> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid) 
        {
            var errors = validationResult.Errors.Select(s => s.ErrorMessage).ToArray();
            return ResultModel.Error(400, errors);
        }

        var profile = await dbContext
            .Accounts
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if(profile is null)
        {
            return ResultModel.Error(400, "Unable to update profile");
        }

        profile.Name = request.Name;
        profile.Email = request.Email;
        profile.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return ResultModel.Success();
    }
}
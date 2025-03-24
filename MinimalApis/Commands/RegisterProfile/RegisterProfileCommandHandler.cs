using MediatR;
using Microsoft.EntityFrameworkCore;
using MinimalApis.Domain;
using MinimalApis.Domain.Accounts;
using MinimalApis.Model;

namespace MinimalApis.Commands.RegisterProfile;

public class RegisterProfileCommandHandler(
    RegisterProfileCommandValidator validator,
    IDbContext dbContext)
    : IRequestHandler<RegisterProfileCommand, ResultModel<RegisterProfileCommandDto>>
{
    public async Task<ResultModel<RegisterProfileCommandDto>> Handle(RegisterProfileCommand request, CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(s => s.ErrorMessage).ToArray();
            return ResultModel<RegisterProfileCommandDto>.Error(400, errors);
        }

        if(await dbContext
            .Accounts
            .AnyAsync(r => r.Phone == request.Phone, cancellationToken)) 
        {
            return ResultModel<RegisterProfileCommandDto>.Error(400, $"Profile already registered with this number:{request.Phone}");
        }

        var account = new Account 
        {
            Name = request.Name,
            Phone = request.Phone,
            Email = request.Email,
            CreatedAt = DateTime.UtcNow,
            Status = AccountStatus.Active
        };
        await dbContext
            .Accounts
            .AddAsync(account, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ResultModel<RegisterProfileCommandDto>.Success(new RegisterProfileCommandDto
        {
            Token = $"{account.Id}",
            ExpiryIn = TimeSpan.FromMinutes(5)
        });
    }
}

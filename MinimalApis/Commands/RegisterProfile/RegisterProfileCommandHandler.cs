using MediatR;
using Microsoft.EntityFrameworkCore;
using MinimalApis.Domain;
using MinimalApis.Domain.Accounts;
using MinimalApis.Model;
using MinimalApis.Services;

namespace MinimalApis.Commands.RegisterProfile;

public class RegisterProfileCommandHandler(
    RegisterProfileCommandValidator validator,
    IEmailService emailService,
    IDbContext dbContext)
    : IRequestHandler<RegisterProfileCommand, ResultModel<RegisterProfileCommandDto>>
{
    public delegate Task RegisteredEventHandler(Account account);
    public event RegisteredEventHandler? RegisteredEvent;
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

        RegisteredEvent += async (account) =>
        {
            await Task.Delay(1000);
            await emailService.Send(toMail: account.Email, subject: "Registed Completed", contents: "");
            await Console.Out.WriteLineAsync("Async event handler finished.");
        };
        await dbContext
            .Accounts
            .AddAsync(account, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        RegisteredEvent?.Invoke(account);

        return ResultModel<RegisterProfileCommandDto>.Success(new RegisterProfileCommandDto
        {
            Token = $"{account.Id}",
            ExpiryIn = TimeSpan.FromMinutes(5)
        });
    }
}

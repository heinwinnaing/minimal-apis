using MediatR;
using Microsoft.EntityFrameworkCore;
using MinimalApis.Domain;
using MinimalApis.Model;

namespace MinimalApis.Commands.RequestOtp;

public class RequestOtpCommandHandler(
    RequestOtpCommandValidator validator,
    IDbContext dbContext)
    : IRequestHandler<RequestOtpCommand, ResultModel<RequestOtpCommandDto>>
{
    public async Task<ResultModel<RequestOtpCommandDto>> Handle(RequestOtpCommand request, CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(s => s.ErrorMessage).ToArray();
            return ResultModel<RequestOtpCommandDto>.Error(400, errors);
        }

        var account = await dbContext
            .Accounts
            .FirstOrDefaultAsync(r => r.Phone == request.Phone, cancellationToken);

        var result = new RequestOtpCommandDto 
        {
            ExpiryIn = TimeSpan.FromMinutes(5),
            Token = $"{account?.Id ?? Guid.Empty}"
        };

        return ResultModel<RequestOtpCommandDto>.Success(result);
    }
}
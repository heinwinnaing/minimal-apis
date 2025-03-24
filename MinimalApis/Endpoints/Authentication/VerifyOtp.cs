using MediatR;
using Microsoft.AspNetCore.Mvc;
using MinimalApis.Commands.VerifyOtp;
using MinimalApis.Model;

namespace MinimalApis.Endpoints.Authentication;

public class VerifyOtp
    : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/verify-otp", async (
            [FromBody]VerifyOtpCommand command,
            CancellationToken cancellationToken,
            IMediator mediator) => 
        {
            cancellationToken.ThrowIfCancellationRequested();
            var result = await mediator.Send(command, cancellationToken);
            if (result.IsSuccess)
                return Results.Ok(result);

            return Results.BadRequest(result);
        })
            .WithTags("Authentication")
            .WithSummary("[verify otp]")
            .HasApiVersion(1)
            .Accepts(typeof(VerifyOtpCommand), "application/json")
            .Produces(200, typeof(ResultModel<VerifyOtpCommandDto>));
    }
}

using MediatR;
using Microsoft.AspNetCore.Mvc;
using MinimalApis.Commands.RequestOtp;
using MinimalApis.Model;

namespace MinimalApis.Endpoints.Authentication;

public class RequestOtp
    : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/request-otp", async (
            [FromBody]RequestOtpCommand command,
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
            .WithSummary("[request otp]")
            .HasApiVersion(1)
            .Accepts(typeof(RequestOtpCommand), "application/json")
            .Produces(200, typeof(ResultModel<RequestOtpCommandDto>));
    }
}


using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinimalApis.Commands.Verify2Fa;
using MinimalApis.Services;
using System.Security.Claims;

namespace MinimalApis.Endpoints.TwoFactor;

public class Verify2Fa : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/two-fa/verify", async (
            [FromQuery] string code,
            HttpContext httpContext,
            IMediator mediator,
            CancellationToken cancellationToken = default) =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            var nameIdentifier = httpContext?.User?.Claims?.FirstOrDefault(r => r.Type == ClaimTypes.NameIdentifier)?.Value;
            _ = Guid.TryParse(nameIdentifier, out Guid accountId);
            var result = await mediator.Send(new Verify2FaCommand(accountId, code), cancellationToken);

            if (result.IsSuccess)
                return Results.Ok(result);

            return Results.BadRequest(result);
        })
            .HasApiVersion(1)
            .WithTags("Two Factor")
            .WithSummary("[Required jwt token for verify two factor authentication]")
            .RequireAuthorization();
    }
}


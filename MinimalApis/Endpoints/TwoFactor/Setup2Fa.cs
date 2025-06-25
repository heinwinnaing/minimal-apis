
using MediatR;
using MinimalApis.Commands.Setup2Fa;
using System.Security.Claims;

namespace MinimalApis.Endpoints.TwoFactor;

public class Setup2Fa : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/two-fa/setup", async (
            HttpContext httpContext,
            IMediator mediator,
            CancellationToken cancellationToken = default) =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            var nameIdentifier = httpContext?.User?.Claims?.FirstOrDefault(r => r.Type == ClaimTypes.NameIdentifier)?.Value;
            _ = Guid.TryParse(nameIdentifier, out Guid accountId);
            var result = await mediator.Send(new Setup2FaCommand(accountId), cancellationToken);

            if (result.IsSuccess)
                return Results.Ok(result);

            return Results.BadRequest(result);
        })
            .HasApiVersion(1)
            .WithTags("Two Factor")
            .WithSummary("[Required jwt token for setup two factor authentication]")
            .RequireAuthorization();
    }
}


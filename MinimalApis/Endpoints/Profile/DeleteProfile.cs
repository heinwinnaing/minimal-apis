using MediatR;
using MinimalApis.Commands.DeleteProfile;
using MinimalApis.Model;
using System.Security.Claims;

namespace MinimalApis.Endpoints.Profile;

public class DeleteProfile
    : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("profile", async (
            HttpContext httpContext,
            CancellationToken cancellationToken,
            IMediator mediator) => 
        {
            cancellationToken.ThrowIfCancellationRequested();

            var nameIdentifier = httpContext?.User?.Claims?.FirstOrDefault(r => r.Type == ClaimTypes.NameIdentifier)?.Value;
            _ = Guid.TryParse(nameIdentifier, out Guid accountId);

            var result = await mediator.Send(new DeleteProfileCommand { }, cancellationToken);
            if (result.IsSuccess) return Results.Ok(result);

            return Results.BadRequest(result);
        })
            .WithTags("Profile")
            .WithSummary("[Required jwt token for delete profile]")
            .HasApiVersion(1)
            .RequireAuthorization()
            .Produces<ResultModel>(200, "application/json");
    }
}

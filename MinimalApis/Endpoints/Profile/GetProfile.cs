
using MediatR;
using MinimalApis.Model;
using MinimalApis.Queries.GetProfile;
using System.Security.Claims;

namespace MinimalApis.Endpoints.Profile;

public class GetProfile
    : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("profile", async (
            CancellationToken cancellationToken,
            HttpContext httpContext,
            IMediator mediator) => 
        {
            cancellationToken.ThrowIfCancellationRequested();

            var nameIdentifier = httpContext?.User?.Claims?.FirstOrDefault(r => r.Type == ClaimTypes.NameIdentifier)?.Value;
            _ = Guid.TryParse(nameIdentifier, out Guid accountId);
            var result = await mediator.Send(new GetProfileQuery { Id = accountId }, cancellationToken);

            if (result.IsSuccess)
                return Results.Ok(result);

            return Results.BadRequest(result);
        })
            .WithTags("Profile")
            .WithSummary("[Required jwt token for get profile]")
            .HasApiVersion(1)
            .Produces<ResultModel<GetProfileDto>>(200)
            .Produces<ResultModel>(400)
            .RequireAuthorization();
    }
}

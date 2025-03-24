using MediatR;
using Microsoft.AspNetCore.Mvc;
using MinimalApis.Commands.UpdateProfile;
using MinimalApis.Model;
using System.Security.Claims;

namespace MinimalApis.Endpoints.Profile;

public class UpdateProfile
    : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("profile", async (
            [FromBody]UpdateProfileCommand request,
            CancellationToken cancellationToken,
            HttpContext httpContext,
            IMediator mediator) => 
        {
            cancellationToken.ThrowIfCancellationRequested();
            var nameIdentifier = httpContext?.User?.Claims?.FirstOrDefault(r => r.Type == ClaimTypes.NameIdentifier)?.Value;
            _ = Guid.TryParse(nameIdentifier, out Guid accountId);
            request.Id = accountId;

            var result = await mediator.Send(request, cancellationToken);

            if (result.IsSuccess)
                return Results.Ok(result);

            return Results.BadRequest(result);
        })
            .WithTags("Profile")
            .WithSummary("[Required jwt token for update profile]")
            .HasApiVersion(1)
            .Accepts(typeof(UpdateProfileCommand), "application/json")
            .Produces<ResultModel>(200)
            .Produces<ResultModel>(400)
            .RequireAuthorization();
    }
}

﻿
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MinimalApis.Commands.RegisterProfile;
using MinimalApis.Model;

namespace MinimalApis.Endpoints.Authentication;

public class RegisterProfile
    : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/register", async (
            [FromBody]RegisterProfileCommand command,
            CancellationToken cancellationToken,
            IMediator mediator) => 
        {
            cancellationToken.ThrowIfCancellationRequested();
            var result = await mediator.Send(command, cancellationToken);
            if(result.IsSuccess) return Results.Ok(result);

            return Results.BadRequest(result);
        })
            .WithTags("Authentication")
            .WithSummary("[Register profile]")
            .HasApiVersion(1)
            .Accepts<RegisterProfileCommand>("application/json")
            .Produces<ResultModel<RegisterProfileCommandDto>>(200);
    }
}

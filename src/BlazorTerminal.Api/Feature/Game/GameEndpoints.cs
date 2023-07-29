﻿namespace BlazorTerminal.Api.Feature.Game;

internal static class GameEndpoints
{
    internal static void MapGameEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        var endpointGroup = routeBuilder
            .MapGroup("/api/game")
            .WithTags("Game")
            .WithOpenApi();
        
        endpointGroup.MapPost("/", (
            [FromServices] ISender sender,
            CancellationToken cancellationToken
        ) => sender.SendAsync(new CreateGameCommand(), cancellationToken))
            .WithName("Start Game Session")
            .WithDescription("Starts a new game session");
    }
}
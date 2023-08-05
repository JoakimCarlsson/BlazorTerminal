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
                [FromBody] CreateGameSessionCommand command,
                [FromServices] ISender sender,
                CancellationToken cancellationToken
            ) => sender.SendAsync(command, cancellationToken))
            .WithName("Start Game Session")
            .WithDescription("Starts a new game session");

        endpointGroup.MapPost("/{GameId:guid}/guess", (
                [AsParameters] GuessWordCommand command,
                [FromServices] ISender sender,
                CancellationToken cancellationToken
            ) => sender.SendAsync(command, cancellationToken))
            .WithName("Guess Word")
            .WithDescription("Starts a new game session");
    }
}
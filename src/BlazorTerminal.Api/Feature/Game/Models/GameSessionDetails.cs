namespace BlazorTerminal.Api.Feature.Game.Models;

internal sealed record GameSessionDetails(
    Guid Id,
    IEnumerable<string> ScrambledWords,
    int AttemptsRemaining
);
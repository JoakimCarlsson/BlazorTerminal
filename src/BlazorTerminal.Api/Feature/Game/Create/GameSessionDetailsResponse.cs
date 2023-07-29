namespace BlazorTerminal.Api.Feature.Game.Create;

internal sealed record GameSessionDetailsResponse(
    Guid Id,
    IEnumerable<string> ScrambledWords,
    int AttemptsRemaining
);
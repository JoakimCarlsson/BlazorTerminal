namespace BlazorTerminal.Client.Models;

internal sealed record GameSessionDetailsResponse(
    Guid Id,
    IEnumerable<string> ScrambledWords,
    int AttemptsRemaining
);
namespace BlazorTerminal.Client.Models;

internal sealed record GameSessionDetails(
    Guid Id,
    IEnumerable<string> ScrambledWords,
    int AttemptsRemaining
);
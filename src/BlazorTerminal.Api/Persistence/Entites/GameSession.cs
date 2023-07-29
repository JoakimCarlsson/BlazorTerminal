namespace BlazorTerminal.Api.Persistence.Entites;

internal sealed record GameSession(
    Guid Id,
    string CurrentWord,
    IEnumerable<string> ScrambledWords,
    int AttemptsRemaining,
    string Status
);
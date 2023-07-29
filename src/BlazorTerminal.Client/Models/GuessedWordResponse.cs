namespace BlazorTerminal.Client.Models;

internal sealed record GuessedWordResponse(
    bool IsCorrect,
    string GuessedWord,
    int CorrectLetters,
    int RemainingAttempts
);
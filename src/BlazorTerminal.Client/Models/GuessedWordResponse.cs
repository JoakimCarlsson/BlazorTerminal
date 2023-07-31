namespace BlazorTerminal.Client.Models;

internal sealed record GuessedWordResponse(
    bool IsCorrect,
    bool IsGameOver,
    string GuessedWord,
    int CorrectLetters,
    int RemainingAttempts
);
namespace BlazorTerminal.Api.Feature.Game.Guess;

internal sealed record GuessedWordResponse(
    bool IsCorrect,
    bool IsGameOver,
    string GuessedWord,
    int CorrectLetters,
    int RemainingAttempts
);
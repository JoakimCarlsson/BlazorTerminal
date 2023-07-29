namespace BlazorTerminal.Api.Feature.Game.Guess;

internal sealed record GuessedWordResponse(
    bool IsCorrect,
    string GuessedWord,
    int CorrectLetters,
    int RemainingAttempts
);
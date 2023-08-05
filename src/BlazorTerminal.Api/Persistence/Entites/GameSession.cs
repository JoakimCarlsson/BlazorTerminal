namespace BlazorTerminal.Api.Persistence.Entites;

internal sealed record GameSession(
    Guid Id,
    string CorrectWord,
    GameBoard[][] Board
)
{
    public int AttemptsRemaining { get; set; }
    public string Status { get; set; }
}

internal sealed class GameBoard
{
    public char Character { get; set; }
    public string? Word { get; set; }
}
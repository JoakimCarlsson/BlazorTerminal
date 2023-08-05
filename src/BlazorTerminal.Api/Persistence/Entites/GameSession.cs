namespace BlazorTerminal.Api.Persistence.Entites;

internal sealed record GameSession(
    Guid Id,
    string CorrectWord,
    Cell[][] Board
)
{
    public int AttemptsRemaining { get; set; }
    public string Status { get; set; }
}
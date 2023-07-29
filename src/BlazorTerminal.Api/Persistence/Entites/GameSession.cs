namespace BlazorTerminal.Api.Persistence.Entites;

internal sealed record GameSession(
    Guid Id,
    string CurrentWord,
    IEnumerable<string> ScrambledWords
)
{
    public int AttemptsRemaining { get; set; }
    public string Status { get; set; }
}
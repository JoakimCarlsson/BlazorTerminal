namespace BlazorTerminal.Client.States;

internal sealed record GameSessionState(
    Guid Id,
    Cell[][] Board,
    int AttemptsRemaining,
    bool IsLoading,
    bool IsGameOver,
    bool IsGameWon,
    List<string> DialogMessages
)
{
    public static GameSessionState Initializing()
    {
        return new GameSessionState
        (
            Guid.Empty,
            Array.Empty<Cell[]>(),
            0,
            false,
            false,
            false,
            new List<string>()
        );
    }
}

internal class Cell
{
    public char Character { get; set; }
    public string? Word { get; set; }
}
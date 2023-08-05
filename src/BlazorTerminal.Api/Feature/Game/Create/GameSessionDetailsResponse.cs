namespace BlazorTerminal.Api.Feature.Game.Create;

internal sealed record GameSessionDetailsResponse(
    Guid Id,
    Cell[][] Board,
    int AttemptsRemaining
);

internal sealed class Cell
{
    public char Character { get; set; }
    public string? Word { get; set; }
}
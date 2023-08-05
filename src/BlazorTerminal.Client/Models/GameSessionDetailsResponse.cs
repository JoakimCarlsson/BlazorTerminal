namespace BlazorTerminal.Client.Models;

internal sealed record GameSessionDetailsResponse(
    Guid Id,
    CellResponse[][] Board,
    int AttemptsRemaining, 
    string Status //use enum
);

internal sealed class CellResponse
{
    public char Character { get; set; }
    public string? Word { get; set; }
}
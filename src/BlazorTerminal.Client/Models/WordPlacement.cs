namespace BlazorTerminal.Client.Models;

public class WordPlacement
{
    public string Word { get; init; }
    public int StartRow { get; init; }
    public int StartColumn { get; init; }
}

public class GridWord
{
    public WordPlacement Placement { get; init; }
    public string Word => Placement.Word;
    public int StartRow => Placement.StartRow;
    public int StartColumn => Placement.StartColumn;
    public int EndColumn => StartColumn + Word.Length - 1;
}
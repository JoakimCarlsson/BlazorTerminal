namespace BlazorTerminal.Client.Models;

public class WordPlacement
{
    public string Word { get; set; }
    public int StartRow { get; set; }
    public int StartColumn { get; set; }
}

public class GridWord
{
    public WordPlacement Placement { get; set; }
    public string Word => Placement.Word;
    public int StartRow => Placement.StartRow;
    public int StartColumn => Placement.StartColumn;
    public int EndColumn => StartColumn + Word.Length - 1;
}
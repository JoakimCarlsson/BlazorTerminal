namespace BlazorTerminal.Client.Pages;

public partial class Index : IDisposable
{
    [Inject] private BlazorTerminalApiService BlazorTerminalApiService { get; set; } = default!;
    
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private GameSessionDetails? _gameSessionDetails;
    
    private readonly Random _random = new();
    private readonly char[][] _characterGrid = new char[20][];
    private readonly List<WordPlacement> _wordPlacements = new();
    private readonly List<GridWord> _gridWords = new();
    
    private const int _gridWidth = 40;
    private const int _gridHeight = 20;
    
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        _gameSessionDetails = await BlazorTerminalApiService.InitializeGameSessionAsync(_cancellationTokenSource.Token);

        if (_gameSessionDetails is null)
            return;
        
        InitializeGrid();
        FillGridWithWords();
        FillGridWithRandomCharacters();
    }

    private void InitializeGrid()
    {
        for (int i = 0; i < _gridHeight; i++)
        {
            _characterGrid[i] = new char[_gridWidth];
        }
    }

    private void FillGridWithWords()
    {
        foreach (var word in _gameSessionDetails.ScrambledWords)
        {
            var row = _random.Next(0, _gridHeight);
            var column = _random.Next(0, _gridWidth - word.Length);

            var wordPlacement = new WordPlacement
            {
                Word = word,
                StartRow = row,
                StartColumn = column
            };

            _wordPlacements.Add(wordPlacement);

            for (int i = 0; i < word.Length; i++)
            {
                _characterGrid[row][column + i] = word[i];
            }

            _gridWords.Add(new GridWord { Placement = wordPlacement });
        }
    }

    private void FillGridWithRandomCharacters()
    {
        var possibleCharacters = ".,!@#$%^&*()-_=+[]{}|;:'\"/?<>`~".ToCharArray();
        for (int i = 0; i < _gridHeight; i++)
        {
            for (int j = 0; j < _gridWidth; j++)
            {
                if (_characterGrid[i][j] == default)
                {
                    var randomCharacter = possibleCharacters[_random.Next(0, possibleCharacters.Length)];
                }
            }
        }
    }
    
    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
    }
}
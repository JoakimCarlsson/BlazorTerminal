namespace BlazorTerminal.Client.Pages;

public partial class Index : IDisposable
{
    [Inject] private BlazorTerminalApiService BlazorTerminalApiService { get; set; } = default!;

    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private GameSessionDetailsResponse? _gameSessionDetails;

    private readonly Random _random = new();
    private readonly char[][] _characterGrid = new char[20][];
    private readonly List<WordPlacement> _wordPlacements = new();
    private readonly List<GridWord> _gridWords = new();
    private readonly List<string> _resultTexts = new();

    private const int GridWidth = 40;
    private const int GridHeight = 20;

    private string _inputText = string.Empty;
    private int _attemptsRemaining = 0;
    
    private bool _guessedRight = false;
    
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        _gameSessionDetails = await BlazorTerminalApiService.InitializeGameSessionAsync(_cancellationTokenSource.Token);

        if (_gameSessionDetails is null)
            return;
        
        _attemptsRemaining = _gameSessionDetails.AttemptsRemaining;
        
        InitializeGrid();
        FillGridWithWords();
        FillGridWithRandomCharacters();
    }

    private void InitializeGrid()
    {
        for (int i = 0; i < GridHeight; i++)
        {
            _characterGrid[i] = new char[GridWidth];
        }
    }

    private void FillGridWithWords()
    {
        foreach (var word in _gameSessionDetails.ScrambledWords)
        {
            var row = _random.Next(0, GridHeight);
            var column = _random.Next(0, GridWidth - word.Length);

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
        for (int i = 0; i < GridHeight; i++)
        {
            for (int j = 0; j < GridWidth; j++)
            {
                if (_characterGrid[i][j] == default)
                {
                    _characterGrid[i][j] = possibleCharacters[_random.Next(0, possibleCharacters.Length)];
                }
            }
        }
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
    }

    private void ShowHoveredText(string text)
    {
        _inputText = text;
    }

    private async Task GuessWordAsync(string word)
    {
        var response = await BlazorTerminalApiService.GuessWordAsync(
            _gameSessionDetails!.Id,
            word,
            _cancellationTokenSource.Token
        );
        
        
        
        if (response.IsCorrect is false)
        {
            _attemptsRemaining = response.RemainingAttempts;
            _resultTexts.Add($"Entry is incorrect, likeliness: {response.CorrectLetters}");
        }

        if (response.IsCorrect)
        {
            _guessedRight = true;
        }
    }
}
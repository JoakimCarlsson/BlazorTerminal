namespace BlazorTerminal.Client.Pages;

public partial class Index : IDisposable
{
    [Inject] private BlazorTerminalApiService BlazorTerminalApiService { get; set; } = default!;
    
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private GameSessionDetails? _gameSessionDetails;
    
    private char[][] _characterGrid = new char[20][];
    private List<WordPlacement> _wordPlacements = new();
    
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        _gameSessionDetails = await BlazorTerminalApiService.InitializeGameSessionAsync(_cancellationTokenSource.Token);

        if (_gameSessionDetails is null)
            return;

        for (var i = 0; i < 20; i++)
        {
            _characterGrid[i] = new char[20 * 5];
        }

        var random = new Random();
        foreach (var word in _gameSessionDetails.ScrambledWords)
        {
            var row = random.Next(0, 20);
            var column = random.Next(0, 20) * 5;

            _wordPlacements.Add(new WordPlacement
            {
                Word = word.PadRight(5, '\0'),
                StartRow = row,
                StartColumn = column
            });

            for (var i = 0; i < 5; i++)
            {
                _characterGrid[row][column + i] = i < word.Length ? word[i] : ' ';
            }
        }

        var possibleCharacters = ".,!@#$%^&*()-_=+[]{}|;:'\"/?<>`~".ToCharArray();
        for (var i = 0; i < 20; i++)
        {
            for (var j = 0; j < 20 * 5; j++)
            {
                if (_characterGrid[i][j] == '\0')
                {
                    _characterGrid[i][j] = possibleCharacters[random.Next(possibleCharacters.Length)];
                }
            }
        }
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
    }
}
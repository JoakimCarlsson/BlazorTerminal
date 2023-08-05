namespace BlazorTerminal.Client.Pages;

public partial class Index : IDisposable
{
    [Inject] private BlazorTerminalApiService BlazorTerminalApiService { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private GameSessionDetailsResponse? _gameSessionDetails;

    private readonly List<string> _resultTexts = new();
    
    private string _hoveredText = string.Empty;
    private int _attemptsRemaining;
    private bool _guessedRight;
    private bool _gameOver;
    
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        _gameSessionDetails = await BlazorTerminalApiService.InitializeGameSessionAsync(_cancellationTokenSource.Token);

        if (_gameSessionDetails is null)
            return;
        
        _attemptsRemaining = _gameSessionDetails.AttemptsRemaining;
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
    }

    private void ShowHoveredText(string text)
    {
        _hoveredText = text;
    }

    private async Task GuessWordAsync(string word)
    {
        var response = await BlazorTerminalApiService.GuessWordAsync(
            _gameSessionDetails!.Id,
            word,
            _cancellationTokenSource.Token
        );

        if (response.IsGameOver)
            _gameOver = true;
        
        if (response.IsCorrect is false)
        {
            _attemptsRemaining = response.RemainingAttempts;
            _resultTexts.Add($"Entry is incorrect, likeliness: {response.CorrectLetters}");
        }

        if (response.IsCorrect)
            _guessedRight = true;
    }
}
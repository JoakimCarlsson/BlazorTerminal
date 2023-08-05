namespace BlazorTerminal.Client.Pages;

public partial class Game : IDisposable
{
    [Inject] private GameSessionStore GameSessionStore { get; set; } = default!;
    [Inject] private ISender Sender { get; set; } = default!;
    [Parameter] public string Id { get; set; } = string.Empty;
    
    private GameSessionState GameSessionState => GameSessionStore.State;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private string _hoveredText = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        if (GameSessionState.Id == Guid.Empty)
        {
            if (Guid.TryParse(Id, out var guid))
                await Sender.SendAsync(new GetGameSessionCommand(guid), _cancellationTokenSource.Token);
        }
        GameSessionStore.OnChange += StateHasChanged;
    }

    public void Dispose() => _cancellationTokenSource.Cancel();
    private void ShowHoveredText(string text) => _hoveredText = text;
    private Task GuessWordAsync(string word) => Sender.SendAsync(new GuessWordCommand(word));
}
namespace BlazorTerminal.Client.Pages;

public partial class Index : IDisposable
{
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private ISender Sender { get; set; } = default!;
    [Inject] private GameSessionStore GameSessionStore { get; set; } = default!;
    
    private GameSessionState GameSessionState => GameSessionStore.State;
    
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    
    private async Task CreateGameSessionAsync(GameDifficulty difficulty) => await Sender.SendAsync(new CreateGameSessionCommand(difficulty), _cancellationTokenSource.Token);
    public void Dispose() => _cancellationTokenSource.Dispose();
    
    protected override void OnInitialized() => GameSessionStore.OnChange += StateHasChanged;
}
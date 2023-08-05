namespace BlazorTerminal.Client.Pages;

public partial class Index : IDisposable
{
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private ISender Sender { get; set; } = default!;
    
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    
    private async Task CreateGameSessionAsync(GameDifficulty difficulty) => await Sender.SendAsync(new CreateGameSessionCommand(difficulty), _cancellationTokenSource.Token);
    public void Dispose() => _cancellationTokenSource.Dispose();
}
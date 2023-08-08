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
    
    private RenderFragment GenerateTerminalText()
    {
        return builder =>
        {
            var messages = new List<string>
            {
                "Bypassing the mainframe...",
                "Decrypting enemy cipher...",
                "Overriding security protocols...",
                "Synchronizing the neuro-algorithm...",
                "Scanning network topology...",
                "Establishing secure uplink...",
                "Injecting payload into target's database...",
                "Retrieving encrypted data packets...",
                "Uploading trojan horse...",
                "Disabling enemy firewalls...",
                "Activating dark web channels...",
                "Initializing stealth VPN connection...",
                "Compiling next-gen decompiler...",
                "Intercepting server handshake signals...",
                "Randomizing IP address...",
                "Navigating through digital minefield...",
                "Hacking into the mainframe...",
            };

            var random = new Random();
            var numberOfLines = random.Next(10, messages.Count - 1);

            var sequence = 0;
            for (var i = 0; i < numberOfLines; i++)
            {
                builder.AddContent(sequence++, messages[random.Next(messages.Count)] + "\n");
            }
        };
    }
}
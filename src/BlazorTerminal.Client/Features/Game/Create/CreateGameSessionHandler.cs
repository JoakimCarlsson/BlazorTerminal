namespace BlazorTerminal.Client.Features.Game.Create;

internal sealed record CreateGameSessionCommand(GameDifficulty Difficulty) : IRequest;

internal class CreateGameSessionHandler : IHandler<CreateGameSessionCommand>
{
    private readonly BlazorTerminalApiService _blazorTerminalApiService;
    private readonly GameSessionStore _gameSessionStore;
    private readonly NavigationManager _navigationManager;

    public CreateGameSessionHandler(
        BlazorTerminalApiService blazorTerminalApiService,
        GameSessionStore gameSessionStateContainer,
        NavigationManager navigationManager
    )
    {
        _blazorTerminalApiService = blazorTerminalApiService;
        _gameSessionStore = gameSessionStateContainer;
        _navigationManager = navigationManager;
    }

    public async Task HandleAsync(
        CreateGameSessionCommand request,
        CancellationToken cancellationToken = default
        )
    {
        _gameSessionStore.SetState(GameSessionState.Initializing());
        
        var response = await _blazorTerminalApiService.InitializeGameSessionAsync(
            request.Difficulty,
            cancellationToken
        );
        
        if (response is null)
           return; //do something here
        
        _gameSessionStore.SetState(new GameSessionState
        (
            response.Id,
            response.Board.Select(row => row.Select(cell => new Cell
            {
                Character = cell.Character,
                Word = cell.Word
            }).ToArray()).ToArray(),
            response.AttemptsRemaining,
            false,
            false,
            false,
            new List<string>()
        ));
        
        _navigationManager.NavigateTo($"/game/{response.Id}");
    }
}
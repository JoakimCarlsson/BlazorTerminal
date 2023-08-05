namespace BlazorTerminal.Client.Features.Game.Get;

internal sealed record GetGameSessionCommand(Guid Id) : IRequest;

internal sealed class GetGameSessionHandler : IHandler<GetGameSessionCommand> 
{
    private readonly BlazorTerminalApiService _blazorTerminalApiService;
    private readonly GameSessionStore _gameSessionStore;

    public GetGameSessionHandler(
        BlazorTerminalApiService blazorTerminalApiService,
        GameSessionStore gameSessionStore,
        NavigationManager navigationManager
        )
    {
        _blazorTerminalApiService = blazorTerminalApiService;
        _gameSessionStore = gameSessionStore;
    }
    
    public async Task HandleAsync(GetGameSessionCommand request, CancellationToken cancellationToken = default)
    {
        _gameSessionStore.SetState(GameSessionState.Initializing());
        var response = await _blazorTerminalApiService.GetGameSessionAsync(request.Id, cancellationToken);
        if(response is null)
            return; //do something here, if 404, not found if error etc. 
        
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
            response.AttemptsRemaining == 0,
            response.Status == "Completed",
            new List<string>()
        ));
    }
}
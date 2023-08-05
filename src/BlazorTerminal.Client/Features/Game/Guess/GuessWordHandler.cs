namespace BlazorTerminal.Client.Features.Game.Guess;

internal sealed record GuessWordCommand(string Word) : IRequest;

internal sealed class GuessWordHandler : IHandler<GuessWordCommand>
{
    private readonly BlazorTerminalApiService _blazorTerminalApiService;
    private readonly GameSessionStore _gameSessionStore;

    public GuessWordHandler(
        BlazorTerminalApiService blazorTerminalApiService,
        GameSessionStore gameSessionStore
        )
    {
        _blazorTerminalApiService = blazorTerminalApiService;
        _gameSessionStore = gameSessionStore;
    }
    
    public async Task HandleAsync(GuessWordCommand request, CancellationToken cancellationToken = default)
    {
        var oldState = _gameSessionStore.State;
    
        var response = await _blazorTerminalApiService.GuessWordAsync(oldState.Id, request.Word, cancellationToken);
        if (response is null)
            return; //do something

        var dialogMessages = new List<string>(oldState.DialogMessages)
        {
            $" > Entry is incorrect, likeliness: {response.CorrectLetters}"
        };

        var newState = oldState with
        {
            AttemptsRemaining = response.RemainingAttempts,
            IsGameOver = response.IsGameOver,
            IsGameWon = response.IsCorrect,
            DialogMessages = dialogMessages
        };
    
        _gameSessionStore.SetState(newState);
    }
}
namespace BlazorTerminal.Api.Feature.Game.Get;

internal sealed record GetGameSessionCommand(
    [FromRoute] Guid GameId
) : IRequest<Results<Ok<GameSessionDetailsResponse>, NotFound>>;

internal sealed class GetGameSessionHandler : IHandler<GetGameSessionCommand, Results<Ok<GameSessionDetailsResponse>, NotFound>>
{
    private readonly IGameSessionsRepository _gameSessionsRepository;
    private readonly IDistributedCache _distributedCache;

    public GetGameSessionHandler(
        IGameSessionsRepository gameSessionsRepository,
        IDistributedCache distributedCache
    )
    {
        _gameSessionsRepository = gameSessionsRepository;
        _distributedCache = distributedCache;
    }

    public async Task<Results<Ok<GameSessionDetailsResponse>, NotFound>> HandleAsync(
        GetGameSessionCommand request,
        CancellationToken cancellationToken = default
    )
    {
        var gameSession = await GetGameSessionAsync(request.GameId);

        if (gameSession is null)
            return TypedResults.NotFound();

        return TypedResults.Ok(new GameSessionDetailsResponse(
            gameSession.Id,
            gameSession.Board.Select(row => row.Select(cell => new Cell
            {
                Character = cell.Character,
                Word = cell.Word
            }).ToArray()).ToArray(),
            gameSession.AttemptsRemaining
        ));
    }

    private async Task<GameSession?> GetGameSessionAsync(Guid gameId)
    {
        var gameSession = await _distributedCache.GetAsync<GameSession>(gameId.ToString());
        if (gameSession is not null)
            return gameSession;

        return await _gameSessionsRepository.GetAsync(gameId);
    }
}
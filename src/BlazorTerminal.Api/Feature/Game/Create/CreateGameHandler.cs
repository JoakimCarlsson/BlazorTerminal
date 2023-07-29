namespace BlazorTerminal.Api.Feature.Game.Create;

internal sealed record CreateGameCommand : IRequest<Results<Created<GameSessionDetails>, BadRequest>>;

internal sealed class CreateGameHandler : IHandler<CreateGameCommand, Results<Created<GameSessionDetails>, BadRequest>>
{
    private readonly IGameSessionsRepository _gameSessionsRepository;
    private readonly IDistributedCache _distributedCache;

    public CreateGameHandler(
        IGameSessionsRepository gameSessionsRepository,
        IDistributedCache distributedCache
        )
    {
        _gameSessionsRepository = gameSessionsRepository;
        _distributedCache = distributedCache;
    }

    public async Task<Results<Created<GameSessionDetails>, BadRequest>> HandleAsync(CreateGameCommand request,
        CancellationToken cancellationToken = default)
    {
        var random = new Random();
        IEnumerable<string> scrambledWords = new List<string> { "word1", "word2", "word3", "word4", "word5" };
        
        var gameSession = new GameSession(
            Guid.NewGuid(),
            scrambledWords.ElementAt(random.Next(0, scrambledWords.Count())),
            scrambledWords,
            5,
            "In Progress"
        );

        var createdSession = await _gameSessionsRepository.CreateAsync(gameSession, cancellationToken);

        await _distributedCache.SetAsync(
            createdSession.Id.ToString(),
            createdSession,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
            },
            cancellationToken);
        
        return TypedResults.Created(
            $"api/game/{createdSession.Id}",
            new GameSessionDetails(
                createdSession.Id,
                createdSession.ScrambledWords,
                createdSession.AttemptsRemaining
            )
        );
    }
}
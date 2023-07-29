namespace BlazorTerminal.Api.Feature.Game.Guess;

internal sealed record GuessWordCommand(
    [FromRoute] Guid GameId,
    [FromBody] GuessWordModel Body
) : IRequest<Results<Ok<GuessedWordResponse>, NotFound, BadRequest>>;

internal sealed record GuessWordModel(
    string Word
);

internal sealed class GuessWordCommandHandler : IHandler<GuessWordCommand, Results<Ok<GuessedWordResponse>, NotFound, BadRequest>>
{
    private readonly IDistributedCache _distributedCache;
    private readonly IGameSessionsRepository _gameSessionsRepository;

    public GuessWordCommandHandler(
        IDistributedCache distributedCache, 
        IGameSessionsRepository gameSessionsRepository
        )
    {
        _distributedCache = distributedCache;
        _gameSessionsRepository = gameSessionsRepository;
    }
    
    public async Task<Results<Ok<GuessedWordResponse>, NotFound, BadRequest>> HandleAsync(
        GuessWordCommand request,
        CancellationToken cancellationToken = default
        )
    {
        request.Deconstruct(out var gameId, out var model);

        var gameSession = await GetGameSessionAsync(gameId);
        if (gameSession is null)
            return TypedResults.NotFound();

        if (gameSession.Status != "In Progress") //TODO: use enum
            return TypedResults.BadRequest(); //TODO: return a more meaningful error message
        
        var likenessScore = CalculateLikenessScore(model.Word, gameSession.CurrentWord);
        if (likenessScore == gameSession.CurrentWord.Length)
        {
            gameSession.Status = "Completed";
            await UpdateGameSessionAsync(gameSession, cancellationToken);
            
            return TypedResults.Ok(new GuessedWordResponse(
                true,
                model.Word,
                likenessScore,
                gameSession.AttemptsRemaining
            ));
        }
        
        gameSession.AttemptsRemaining--;
        await UpdateGameSessionAsync(gameSession, cancellationToken);
        
        return TypedResults.Ok(new GuessedWordResponse(
            false,
            model.Word,
            likenessScore,
            gameSession.AttemptsRemaining
        ));
    }

    private async Task UpdateGameSessionAsync(
        GameSession gameSession, 
        CancellationToken cancellationToken
        )
    {
        await _gameSessionsRepository.UpdateAsync(gameSession, cancellationToken);
        await _distributedCache.SetAsync(
            gameSession.Id.ToString(),
            gameSession,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
            },
            cancellationToken);
    }

    private async Task<GameSession?> GetGameSessionAsync(Guid gameId)
    {
        var gameSession = await _distributedCache.GetAsync<GameSession>(gameId.ToString());
        if (gameSession is not null)
            return gameSession;
        
        return await _gameSessionsRepository.GetAsync(gameId);
    }
    
    private int CalculateLikenessScore(
        string guessWord, 
        string currentWord
        )
    {
        var score = 0;
        for (var i = 0; i < Math.Min(guessWord.Length, currentWord.Length); i++)
        {
            if (guessWord[i] == currentWord[i])
            {
                score++;
            }
        }
        return score;
    }
}
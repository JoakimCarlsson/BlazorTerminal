namespace BlazorTerminal.Api.Feature.Game.Create;

internal sealed record CreateGameCommand : IRequest<Results<Created<GameSessionDetailsResponse>, BadRequest>>;

internal sealed class
    CreateGameHandler : IHandler<CreateGameCommand, Results<Created<GameSessionDetailsResponse>, BadRequest>>
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

    public async Task<Results<Created<GameSessionDetailsResponse>, BadRequest>> HandleAsync(CreateGameCommand request,
        CancellationToken cancellationToken = default)
    {
        var random = new Random();
        var scrambledWords = GenerateWords(10, 5).ToList();

        var gameSession = new GameSession(
            Guid.NewGuid(),
            scrambledWords.ElementAt(random.Next(0, scrambledWords.Count())),
            scrambledWords
        )
        {
            AttemptsRemaining = 5,
            Status = "In Progress"
        };

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
            new GameSessionDetailsResponse(
                createdSession.Id,
                createdSession.ScrambledWords,
                createdSession.AttemptsRemaining
            )
        );
    }

    //get a list of words from somewhere else :-)
    private IEnumerable<string> GenerateWords(int wordCount, int wordLength)
    {
        var allWords = GetWords();

        var suitableWords = allWords.Where(word => word.Length == wordLength).ToList();
        var random = new Random();

        var selectedWords = new List<string>();
        for (var i = 0; i < wordCount; i++)
        {
            var randomIndex = random.Next(suitableWords.Count);
            selectedWords.Add(suitableWords[randomIndex]);
            suitableWords.RemoveAt(randomIndex);
        }

        return selectedWords;
    }

    private IEnumerable<string> GetWords()
    {
        return new List<string>
        {
            "ABOUT", "AFTER", "APPLE", "ARISE", "BASIC", "BLAME", "BRICK", "CHAIR", "CHIEF", "CHILD",
            "CLAIM", "CLASS", "CLEAR", "CLOCK", "COULD", "CROSS", "DANCE", "DRINK", "EARTH", "EIGHT",
            "EQUAL", "ERROR", "FINAL", "FIRST", "FIXED", "FLAME", "FRESH", "FRUIT", "GLASS", "GUESS",
            "HEART", "HUMAN", "INDEX", "ISSUE", "JUDGE", "KNIFE", "LEARN", "LEGAL", "LEVEL", "LOWER",
            "MAGIC", "MAJOR", "MIGHT", "NEVER", "NOISE", "OCCUR", "OTHER", "POWER", "QUICK", "RIVER",
            "SCALE", "SILENCE", "SINCE", "SWEET", "THANK", "THEIR", "THIRD", "THOSE", "UNDER", "USAGE",
            "VALUE", "VIDEO", "WATCH", "WORLD", "WORTH", "YOUTH", "ZEBRA", "ALLOW", "ALONG", "BELOW"
        };
    }
}
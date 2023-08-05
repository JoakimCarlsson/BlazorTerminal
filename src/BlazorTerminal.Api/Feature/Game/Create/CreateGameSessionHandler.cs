namespace BlazorTerminal.Api.Feature.Game.Create;

internal sealed record CreateGameSessionCommand(GameDifficulty GameDifficulty) : IRequest<Results<Created<GameSessionDetailsResponse>, BadRequest>>;

internal enum GameDifficulty
{
    Easy,
    Medium,
    Hard
}

internal sealed class
    CreateGameSessionHandler : IHandler<CreateGameSessionCommand, Results<Created<GameSessionDetailsResponse>, BadRequest>>
{
    private readonly IGameSessionsRepository _gameSessionsRepository;
    private readonly IDistributedCache _distributedCache;
    private readonly Random _random = new();
    
    private readonly List<string> _words = new()
    {
        "DUST", "WISP", "FLAT", "RAIN", "TINT", "BOLD", "ARCH", "PEAK", "MASK", "GLOW", 
        "CHIP", "JAZZ", "ZONE", "ECHO", "MUTE", "COIL", "QUIZ", "KNOT", "HUSH", "DAZE",
        
        "SCRIBE", "PROPEL", "JIGSAW", "LEGACY", "BOTTLE", "SUMMIT", "KITTEN", "BISHOP", "BINARY", "AMULET", 
        "OPTICS", "PHOBIA", "GRAVEL", "PRIMAL", "STROBE", "CHROME", "VECTOR", "RITUAL", "FIZZLE", "WIDGET",
        
        "ELEPHANT", "TRINKETS", "MONOPOLY", "DORMITORY", "BLUEPRINT", "THEMATICS", "BOOKMARK", "PROLIFIC", "ANTIVIRUS", "AQUEDUCT",
        "NARRATIVE", "HANGOVER", "GLYCERIN", "MANIFEST", "POSITIVE", "MOMENTUM", "OUTRIDER", "PARABOLIC", "SCAFFOLD", "TURBINES"
    };
    
    public CreateGameSessionHandler(
        IGameSessionsRepository gameSessionsRepository,
        IDistributedCache distributedCache
    )
    {
        _gameSessionsRepository = gameSessionsRepository;
        _distributedCache = distributedCache;
    }

    public async Task<Results<Created<GameSessionDetailsResponse>, BadRequest>> HandleAsync(CreateGameSessionCommand request,
        CancellationToken cancellationToken = default)
    {
        var scrambledWords = GenerateWords(10, 5).ToList();

        var gameSession = new GameSession(
            Guid.NewGuid(),
            scrambledWords.ElementAt(_random.Next(0, scrambledWords.Count())),
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

        var selectedWords = new List<string>();
        for (var i = 0; i < wordCount; i++)
        {
            var randomIndex = _random.Next(suitableWords.Count);
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
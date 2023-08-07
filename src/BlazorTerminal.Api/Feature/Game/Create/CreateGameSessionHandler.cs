namespace BlazorTerminal.Api.Feature.Game.Create;

internal sealed record CreateGameSessionCommand(GameDifficulty GameDifficulty) : IRequest<Results<Created<GameSessionDetailsResponse>, BadRequest>>;

internal enum GameDifficulty
{
    Easy,
    Medium,
    Hard
}

internal sealed class
    CreateGameSessionHandler : IHandler<CreateGameSessionCommand,
        Results<Created<GameSessionDetailsResponse>, BadRequest>>
{
    private readonly IGameSessionsRepository _gameSessionsRepository;
    private readonly IDistributedCache _distributedCache;
    private readonly Random _random = new();

    private const int GridWidth = 40;
    private const int GridHeight = 20;

    private readonly Cell[][] _board = new Cell[GridHeight][];
    private readonly char[] _possibleCharacters = ".,!@#$%^&*()-_=+[]{}|;:'\"/?<>`~".ToCharArray();

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

    public async Task<Results<Created<GameSessionDetailsResponse>, BadRequest>> HandleAsync(
        CreateGameSessionCommand request,
        CancellationToken cancellationToken = default
    )
    {
        InitializeGrid();
        var puzzleWords = GetPuzzleWords(request.GameDifficulty).ToList();

        FillGridWithWords(puzzleWords);
        FillGridWithRandomCharacters();

        var gameboard = new GameBoard[GridHeight][];
        for (var i = 0; i < GridHeight; i++)
        {
            gameboard[i] = new GameBoard[GridWidth];
            for (var j = 0; j < GridWidth; j++)
            {
                gameboard[i][j] = new GameBoard
                {
                    Character = _board[i][j].Character,
                    Word = _board[i][j].Word
                };
            }
        }
        var gameSession = new GameSession(
            Guid.NewGuid(),
            puzzleWords[_random.Next(0, puzzleWords.Count)],
            _board.Select(row => row.Select(cell => new GameBoard
            {
                Character = cell.Character,
                Word = cell.Word
            }).ToArray()).ToArray()
        )
        {
            AttemptsRemaining = 5,
            Status = "In Progress"
        };

        await _gameSessionsRepository.CreateAsync(gameSession, cancellationToken);
        await _distributedCache.SetAsync(
            gameSession.Id.ToString(),
            gameSession,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
            },
            cancellationToken);
        
        return TypedResults.Created(
            $"api/game/{gameSession.Id}",
            new GameSessionDetailsResponse(
                gameSession.Id,
                _board,
                gameSession.AttemptsRemaining
            ));
    }

    private void InitializeGrid()
    {
        for (var i = 0; i < GridHeight; i++)
        {
            _board[i] = new Cell[GridWidth];
            for (var j = 0; j < GridWidth; j++)
            {
                _board[i][j] = new Cell { Character = default, Word = default };
            }
        }
    }

    private void FillGridWithWords(IEnumerable<string> words)
    {
        foreach (var word in words)
        {
            bool hasOverlap;
            int row, column;
            do
            {
                hasOverlap = false;
                row = _random.Next(0, GridHeight);
                column = _random.Next(0, GridWidth - word.Length);

                for (var i = 0; i < word.Length; i++)
                {
                    if (_board[row][column + i].Character == default) 
                        continue;
                    
                    hasOverlap = true;
                    break;
                }
            } while (hasOverlap);

            for (var i = 0; i < word.Length; i++)
            {
                _board[row][column + i].Character = word[i];
                _board[row][column + i].Word = word;
            }
        }
    }

    private void FillGridWithRandomCharacters()
    {
        for (var i = 0; i < GridHeight; i++)
        {
            for (var j = 0; j < GridWidth; j++)
            {
                if (_board[i][j].Character == default)
                {
                    _board[i][j].Character = _possibleCharacters[_random.Next(0, _possibleCharacters.Length)];
                }
            }
        }
    }

    private IEnumerable<string> GetPuzzleWords(GameDifficulty difficulty)
    {
        var wordLength = difficulty switch
        {
            GameDifficulty.Easy => 4,
            GameDifficulty.Medium => 6,
            GameDifficulty.Hard => 8,
            _ => throw new ArgumentOutOfRangeException(nameof(difficulty), difficulty, "Invalid game level")
        };

        var wordCount = difficulty switch
        {
            GameDifficulty.Easy => 8,
            GameDifficulty.Medium => 10,
            GameDifficulty.Hard => 12,
            _ => throw new ArgumentOutOfRangeException(nameof(difficulty), difficulty, "Invalid game level")
        };

        var suitableWords = _words.Where(word => word.Length == wordLength).ToList();

        var selectedWords = new List<string>();
        for (var i = 0; i < wordCount; i++)
        {
            var randomIndex = _random.Next(suitableWords.Count);
            selectedWords.Add(suitableWords[randomIndex]);
            suitableWords.RemoveAt(randomIndex);
        }

        return selectedWords;
    }
}
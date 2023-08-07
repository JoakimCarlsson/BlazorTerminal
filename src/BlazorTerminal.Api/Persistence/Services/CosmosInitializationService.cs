namespace BlazorTerminal.Api.Persistence.Services;

internal sealed class CosmosInitializationService : BackgroundService
{
    private readonly CosmosClient _cosmosClient;
    private readonly ILogger<CosmosInitializationService> _logger;

    public CosmosInitializationService(
        CosmosClient cosmosClient,
        ILogger<CosmosInitializationService> logger
    )
    {
        _cosmosClient = cosmosClient;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var partitionKeyPath = $"/{nameof(GameSession.Id).ToLower()}";

        var containerProperties = new ContainerProperties(CosmosGlobals.GameSessionContainerName, partitionKeyPath)
        {
            IndexingPolicy = new IndexingPolicy
            {
                Automatic = true,
                IndexingMode = IndexingMode.Consistent,
                ExcludedPaths = { new ExcludedPath { Path = "/*" } }
            }
        };

        var database = await _cosmosClient.CreateDatabaseIfNotExistsAsync(
            CosmosGlobals.DatabaseName,
            cancellationToken: stoppingToken
        );

        await database.Database.CreateContainerIfNotExistsAsync(
            containerProperties,
            cancellationToken: stoppingToken
        );
    }
}
namespace BlazorTerminal.Api.Persistence.Repository;

internal interface IGameSessionsRepository
{
    public ValueTask<GameSession> CreateAsync(
        GameSession gameSession, 
        CancellationToken cancellationToken = default
        );
    
    public ValueTask<GameSession?> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default
        );
    
    public ValueTask<GameSession> UpdateAsync(
        GameSession gameSession,
        CancellationToken cancellationToken = default
        );
    
    public ValueTask DeleteAsync(
        Guid id, 
        CancellationToken cancellationToken = default
        );
}

internal sealed class GameSessionsRepository : IGameSessionsRepository
{
    private readonly Container _gameSessionsContainer;

    public GameSessionsRepository(CosmosClient cosmosClient)
    {
        _gameSessionsContainer = cosmosClient.GetContainer(
            CosmosGlobals.DatabaseName,
            CosmosGlobals.GameSessionContainerName
            );
    }

    public async ValueTask<GameSession> CreateAsync(GameSession gameSession, CancellationToken cancellationToken = default)
    {
        var cosmosResponse = await _gameSessionsContainer.CreateItemAsync(
            gameSession,
            new PartitionKey(gameSession.Id.ToString()),
            cancellationToken: cancellationToken
        );
        
        return cosmosResponse.Resource;
    }

    public async ValueTask<GameSession?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var cosmosResponse = await _gameSessionsContainer.ReadItemAsync<GameSession>(
                id.ToString(),
                new PartitionKey(id.ToString()),
                cancellationToken: cancellationToken
            );

            return cosmosResponse.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return default;
        }
    }

    public async ValueTask<GameSession> UpdateAsync(GameSession gameSession, CancellationToken cancellationToken = default)
    {
        var cosmosResponse = await _gameSessionsContainer.UpsertItemAsync(
            gameSession,
            new PartitionKey(gameSession.Id.ToString()),
            cancellationToken: cancellationToken
        );
        
        return cosmosResponse.Resource;
    }

    public async ValueTask DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _gameSessionsContainer.DeleteItemAsync<GameSession>(
            id.ToString(),
            new PartitionKey(id.ToString()),
            cancellationToken: cancellationToken
        );
    }
}
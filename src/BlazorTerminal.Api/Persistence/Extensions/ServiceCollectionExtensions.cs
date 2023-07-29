namespace BlazorTerminal.Api.Persistence.Extensions;

internal static class ServiceCollectionExtensions
{
    private static IServiceCollection CosmosClientInitialization(
        this IServiceCollection serviceCollection,
        string connectionString
    )
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentNullException(nameof(connectionString), "No connection string provided for Cosmos DB.");

        serviceCollection.AddSingleton(x => 
        {
            var clientBuilder = new CosmosClientBuilder(connectionString);
            return clientBuilder
                .WithSerializerOptions(new CosmosSerializationOptions
                {
                    Indented = false,
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase,
                })
                .Build();
        });
        return serviceCollection;
    }

    internal static IServiceCollection AddCosmosDb(
        this IServiceCollection serviceCollection, 
        string connectionString
    )
    {
        serviceCollection.CosmosClientInitialization(connectionString);
        serviceCollection.AddHostedService<CosmosInitializationService>();

        serviceCollection.AddSingleton<IGameSessionsRepository, GameSessionsRepository>();
        
        return serviceCollection;
    }
}
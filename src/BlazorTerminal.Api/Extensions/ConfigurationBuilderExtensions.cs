namespace BlazorTerminal.Api.Extensions;

internal static class ConfigurationBuilderExtensions
{
    internal static IConfigurationBuilder AddAzureConfiguration(this IConfigurationBuilder configurationBuilder)
    {
        const string appConfigName = "AZURE_APPCONFIG_CONNECTIONSTRING";

        var connectionString = Environment.GetEnvironmentVariable(appConfigName);
        if (string.IsNullOrEmpty(connectionString))
        {
            var configurationRoot = configurationBuilder.Build();
            connectionString = configurationRoot.GetConnectionString(appConfigName);
        }

        configurationBuilder.AddAzureAppConfiguration(options =>
        {
            options
                .Connect(connectionString)
                .ConfigureKeyVault(kv => kv.SetCredential(new DefaultAzureCredential()));

            options
                .Select(KeyFilter.Any)
                .Select(KeyFilter.Any, Environment.MachineName);
        });

        return configurationBuilder;
    }
}
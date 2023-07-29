namespace BlazorTerminal.Client.HttpClients;

internal class BlazorTerminalApiService
{
    private readonly HttpClient _httpClient;
    
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    
    public BlazorTerminalApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    internal async Task<GameSessionDetails?> InitializeGameSessionAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsync(
            "/api/game", 
            null,
            cancellationToken
            );
        
        if (!response.IsSuccessStatusCode)
            return null;
        
        var gameSessionDetails = await response.Content.ReadFromJsonAsync<GameSessionDetails>(
            JsonSerializerOptions,
            cancellationToken: cancellationToken
            );
        
        return gameSessionDetails;
    }
}
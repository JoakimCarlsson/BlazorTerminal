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

    internal async Task<GameSessionDetailsResponse?> GetGameSessionAsync(
        Guid id,
        CancellationToken cancellationToken = default
        )
    {
        var response = await _httpClient.GetAsync($"/api/game/{id}", cancellationToken);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<GameSessionDetailsResponse>(
            JsonSerializerOptions,
            cancellationToken
        );
    }
    
    internal async Task<GameSessionDetailsResponse?> InitializeGameSessionAsync(
        GameDifficulty difficulty,
        CancellationToken cancellationToken = default
        )
    {
        var response = await _httpClient.PostAsJsonAsync(
            "/api/game",
            new
            {
                GameDifficulty = (int)difficulty
            }, JsonSerializerOptions,cancellationToken
        );

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<GameSessionDetailsResponse>(
            JsonSerializerOptions,
            cancellationToken
        );
    }

    internal async Task<GuessedWordResponse?> GuessWordAsync(
        Guid id,
        string word,
        CancellationToken cancellationToken = default
    )
    {
        var response = await _httpClient.PostAsJsonAsync($"/api/game/{id}/guess", new
        {
            Word = word
        }, JsonSerializerOptions, cancellationToken);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<GuessedWordResponse>(
            JsonSerializerOptions,
            cancellationToken: cancellationToken
        );
    }
}
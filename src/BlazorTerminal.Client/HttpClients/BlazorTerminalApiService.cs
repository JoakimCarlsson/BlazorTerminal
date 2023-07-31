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

    internal async Task<GameSessionDetailsResponse?> InitializeGameSessionAsync(
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsync(
            "/api/game",
            null,
            cancellationToken
        );

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<GameSessionDetailsResponse>(
            JsonSerializerOptions,
            cancellationToken: cancellationToken
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
        }, JsonSerializerOptions, cancellationToken: cancellationToken);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<GuessedWordResponse>(
            JsonSerializerOptions,
            cancellationToken: cancellationToken
        );
    }
}
var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBroker();
builder.Services.AddSingleton<GameSessionStore>();

var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .OrResult(response => response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

var circuitBreakerPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));

builder.Services.AddHttpClient<BlazorTerminalApiService>(client =>
    {
        var baseAddress = builder.Configuration.GetValue<string>("apiBaseUrl");
        if (string.IsNullOrWhiteSpace(baseAddress))
            throw new ArgumentNullException(baseAddress, "apiBaseUrl is not set in appsettings.json");
        
        client.BaseAddress = new Uri(baseAddress);
    })
    .AddPolicyHandler(retryPolicy)
    .AddPolicyHandler(circuitBreakerPolicy);

await builder
    .Build()
    .RunAsync();
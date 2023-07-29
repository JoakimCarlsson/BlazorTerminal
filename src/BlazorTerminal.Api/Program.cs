var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCosmosDb("AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");
builder.Services.AddBroker();

var app = builder.Build();

app.UseHttpsRedirection();
app.Run();
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

builder.Services.AddBroker();

app.UseHttpsRedirection();
app.Run();
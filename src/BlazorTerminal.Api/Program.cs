var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCosmosDb("AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");
builder.Services.AddBroker();
builder.Services.AddCors();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseCors(policyBuilder => policyBuilder
        .WithOrigins("*")
        .AllowAnyMethod()
        .AllowAnyHeader()
    );
    
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGameEndpoints();

app.UseHttpsRedirection();
app.Run();
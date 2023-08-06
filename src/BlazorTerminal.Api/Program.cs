var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddAzureConfiguration();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCosmosDb(builder.Configuration);

builder.Services.AddBroker();
builder.Services.AddCors();
builder.Services.AddDistributedMemoryCache();

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
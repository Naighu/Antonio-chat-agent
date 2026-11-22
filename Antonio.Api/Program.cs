using Antonio.Api.Endpoints;
using Antonio.Core.Abstractions;
using Antonio.Core.Services;
using Antonio.Infrastructure;
using Antonio.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);  
builder.Services.AddScoped<ChatService>();
builder.Services.AddSingleton<IConversationStore,InMemoryConversationStore>();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/health", () => Results.Ok());
app.MapChatEndpoint();


app.Run();

using Antonio.Api.Dev;
using Antonio.Api.Endpoints;
using Antonio.Core.Abstractions;
using Antonio.Core.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ILlmClient, EchoLlmClient>();
builder.Services.AddScoped<ChatService>();

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

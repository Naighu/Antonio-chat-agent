using System.Net;
using System.Net.Http.Json;
using Antonio.Api.Dtos;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Antonio.Api.Tests;

public class ChatEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task PostChat_ReturnsReply()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/chat", new ChatRequestDto("s1", "hello"));

        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<ChatResponseDto>();
        Assert.Equal("s1", body!.SessionId);
        Assert.Equal("Echo: hello", body.Reply);
    }

    [Fact]
    public async Task PostChat_EmptyMessage_Returns400()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/chat", new ChatRequestDto("s1", "   "));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Health_ReturnsOk()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
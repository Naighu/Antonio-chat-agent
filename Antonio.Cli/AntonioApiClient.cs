using System.Net.Http.Json;

namespace Antonio.Cli;

public sealed class AntonioApiClient(HttpClient http)
{
    public async Task<string> SendAsync(string sessionId, string message, CancellationToken ct)
    {
        using var response = await http.PostAsJsonAsync("chat", new ChatRequest(sessionId, message), ct);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(ct);
            throw new HttpRequestException($"Antonio API returned {(int)response.StatusCode}: {error}");
        }

        var body = await response.Content.ReadFromJsonAsync<ChatResponse>(ct)
            ?? throw new InvalidOperationException("Empty response from Antonio API.");

        return body.Reply;
    }

    // The CLI's own copy of the API contract. It doesn't reference the Api project.
    private sealed record ChatRequest(string SessionId, string Message);
    private sealed record ChatResponse(string SessionId, string Reply);
}
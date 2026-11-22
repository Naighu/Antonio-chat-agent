using System;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Antonio.Core.Abstractions;
using Antonio.Core.Exceptions;
using Antonio.Core.Models;
using Microsoft.Extensions.Options;

namespace Antonio.Infrastructure.Llm.Claude;

public sealed class ClaudeLlmClient(HttpClient http, IOptions<ClaudeLlmOptions> options) : ILlmClient
{
    private static readonly JsonSerializerOptions Json = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull   // don't send "system": null
    };

    async public Task<LlmResponse> CompleteAsync(LlmRequest request, CancellationToken ct = default)
    {
        var body = new ClaudeRequest(
            Model: options.Value.Model,
            MaxTokens: request.MaxTokens,
            Messages: request.Messages
                .Select(m => new ClaudeMessage(m.Role == Role.User ? "user" : "assistant", m.Text))
                .ToList(),
            System: request.SystemPrompt);

        using var response = await PostAsync(body, ct);

        var raw = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
            throw MapError(response.StatusCode, raw);

        var result = await response.Content.ReadFromJsonAsync<ClaudeResponse>(Json, ct)
           ?? throw new LlmException("Empty response from Claude API.");

        return new LlmResponse(
            Text: string.Concat(result.Content.Where(b => b.Type == "text").Select(b => b.Text)),
            Status: result.StopReason switch
            {
                "end_turn" => ResponseStatus.Completed,
                "max_tokens" => ResponseStatus.MaxTokens,
                "tool_use" => ResponseStatus.Execute,
                _ => ResponseStatus.Other
            },
            InputTokens: result.Usage.InputTokens,
            OutputTokens: result.Usage.OutputTokens);
    }
    private async Task<HttpResponseMessage> PostAsync(ClaudeRequest body, CancellationToken ct)
    {
        try
        {
            return await http.PostAsJsonAsync("v1/messages", body, Json, ct);
        }
        catch (HttpRequestException ex)   // no internet, DNS failure, connection refused
        {
            throw new LlmUnavailableException("Could not reach the Claude API.", ex);
        }
    }
    private static LlmException MapError(HttpStatusCode status, string body) => (int)status switch
    {
        401 or 403 => new LlmAuthException($"Claude rejected the API key ({(int)status})."),
        429 => new LlmRateLimitedException("Claude rate limit reached."),
        >= 500 => new LlmUnavailableException($"Claude API error {(int)status}."),   // includes 529 overloaded
        _ => new LlmException($"Claude API returned {(int)status}: {body}")
    };
}

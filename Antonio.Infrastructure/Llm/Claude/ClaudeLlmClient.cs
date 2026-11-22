using System;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Antonio.Core.Abstractions;
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

        using var response = await http.PostAsJsonAsync("v1/messages", body, Json, ct);


         if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(ct);
            throw new HttpRequestException(
                $"Claude API returned {(int)response.StatusCode}: {error}", null, response.StatusCode);
        }

         var result = await response.Content.ReadFromJsonAsync<ClaudeResponse>(Json, ct)
            ?? throw new InvalidOperationException("Empty response from Claude API.");
        var raw = await response.Content.ReadAsStringAsync(ct);

        return new LlmResponse(
            Text: string.Concat(result.Content.Where(b => b.Type == "text").Select(b => b.Text)),
            Status: result.StopReason switch
            {
                "end_turn"   => ResponseStatus.Completed,
                "max_tokens" => ResponseStatus.MaxTokens,
                "tool_use"   => ResponseStatus.Execute,
                _            => ResponseStatus.Other
            },
            InputTokens: result.Usage.InputTokens,
            OutputTokens: result.Usage.OutputTokens);
    }
}

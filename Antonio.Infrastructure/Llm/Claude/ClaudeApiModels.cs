
using System.Text.Json.Serialization;

namespace Antonio.Infrastructure.Llm.Claude;

internal sealed record ClaudeRequest(
    [property: JsonPropertyName("model")]      string Model,
    [property: JsonPropertyName("max_tokens")] int MaxTokens,
    [property: JsonPropertyName("messages")]   IReadOnlyList<ClaudeMessage> Messages,
    [property: JsonPropertyName("system")]     string? System);

internal sealed record ClaudeMessage(
    [property: JsonPropertyName("role")]    string Role,
    [property: JsonPropertyName("content")] string Content);

internal sealed record ClaudeResponse(
    [property: JsonPropertyName("content")]     List<ClaudeContentBlock> Content,
    [property: JsonPropertyName("stop_reason")] string? StopReason,
    [property: JsonPropertyName("usage")]       ClaudeUsage Usage);

internal sealed record ClaudeContentBlock(
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("text")] string? Text);

internal sealed record ClaudeUsage(
    [property: JsonPropertyName("input_tokens")]  int InputTokens,
    [property: JsonPropertyName("output_tokens")] int OutputTokens);
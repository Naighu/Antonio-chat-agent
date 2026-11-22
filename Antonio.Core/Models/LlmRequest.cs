namespace Antonio.Core.Models;

public  sealed record LlmRequest(
    IReadOnlyList<ChatMessage> Messages,
    string? SystemPrompt = null,
    int MaxTokens = 1024
);

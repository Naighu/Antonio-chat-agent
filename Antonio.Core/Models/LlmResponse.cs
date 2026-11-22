namespace Antonio.Core.Models;

public enum ResponseStatus
{
    Completed,
    MaxTokens,
    InProgress, 
    Execute,
    Other
}
public record  LlmResponse(
    string Text,
    ResponseStatus Status,
    int InputTokens,
    int OutputTokens
);

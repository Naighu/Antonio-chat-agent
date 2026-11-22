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
    ResponseStatus Status,
    string Reply,
    int InputTokens,
    int OutputTokens
);

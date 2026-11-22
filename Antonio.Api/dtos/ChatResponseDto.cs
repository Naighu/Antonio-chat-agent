namespace Antonio.Api.dtos;

public record  ChatResponseDto(
    string SessionId,
    string Reply,
    int inputTokens,
    int outputTokens
);

namespace Antonio.Api.Dtos;

public record  ChatResponseDto(
    string SessionId,
    string Reply,
    int InputTokens =0,
    int OutputTokens=0
);

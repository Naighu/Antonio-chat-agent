using Antonio.Core.Abstractions;
using Antonio.Core.Models;

namespace Antonio.Api.Tests.Fakes;

public sealed class EchoLlmClient() : ILlmClient
{
    public LlmRequest? LastRequest { get; private set; }
    public Task<LlmResponse> CompleteAsync(LlmRequest request, CancellationToken ct = default)
    {
     LastRequest = request;
     return Task.FromResult(new LlmResponse(
            Status: ResponseStatus.Completed,
            Text: $"Echo: {request.Messages[^1].Text}",
            InputTokens: 0,
            OutputTokens: 0
        ));  
    }
}

using System;
using Antonio.Core.Abstractions;
using Antonio.Core.Models;

namespace Antonio.Core.Test.Fakes;

public sealed class FakeLlmClient(string Reply) : ILlmClient
{
    public LlmRequest? LastRequest { get; private set; }
    public Task<LlmResponse> CompleteAsync(LlmRequest request, CancellationToken ct = default)
    {
     LastRequest = request;
     return Task.FromResult(new LlmResponse(
            Status: ResponseStatus.Completed,
            Text: Reply,
            InputTokens: 0,
            OutputTokens: 0
        ));  
    }
}

using Antonio.Core.Abstractions;
using Antonio.Core.Models;

namespace Antonio.Core.Test.Fakes;

public sealed class ThrowingLlmClient : ILlmClient
{
    public Task<LlmResponse> CompleteAsync(LlmRequest request, CancellationToken ct = default) =>
        throw new HttpRequestException("Claude is down");
}
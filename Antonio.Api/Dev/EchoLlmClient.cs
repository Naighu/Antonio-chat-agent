

using Antonio.Core.Abstractions;
using Antonio.Core.Models;

namespace Antonio.Api.Dev;

public class EchoLlmClient : ILlmClient
{
    public Task<LlmResponse> CompleteAsync(LlmRequest request, CancellationToken ct = default)
    {
        var last = request.Messages[^1].Text;
        return Task.FromResult(new LlmResponse($"Echo: {last}", ResponseStatus.Completed, 0, 0));
    }
}

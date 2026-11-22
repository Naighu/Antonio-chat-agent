using Antonio.Core.Models;

namespace Antonio.Core.Abstractions;

public interface ILlmClient
{
Task<LlmResponse> CompleteAsync(LlmRequest request, CancellationToken ct = default);
}

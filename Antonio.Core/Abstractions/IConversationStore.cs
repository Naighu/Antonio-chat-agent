using System;
using Antonio.Core.Models;

namespace Antonio.Core.Abstractions;

public interface IConversationStore
{
    Task<IReadOnlyList<ChatMessage>> GetAsync(string sessionId, CancellationToken ct = default);
    Task AppendAsync(string sessionId, IEnumerable<ChatMessage> messages, CancellationToken ct = default);
    Task ClearAsync(string sessionId, CancellationToken ct = default);
}

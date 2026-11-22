using System;
using Antonio.Core.Abstractions;
using Antonio.Core.Models;

namespace Antonio.Core.Test.Fakes;

public class FakeConversationStore : IConversationStore
{
    public Dictionary<string, List<ChatMessage>> Sessions { get; } = new();
    public Task AppendAsync(string sessionId, IEnumerable<ChatMessage> messages, CancellationToken ct = default)
    {
        if (!Sessions.TryGetValue(sessionId, out var list))
            Sessions[sessionId] = list = [];
        list.AddRange(messages);
        return Task.CompletedTask;
    }

    public Task ClearAsync(string sessionId, CancellationToken ct = default)
    {
        Sessions.Remove(sessionId);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<ChatMessage>> GetAsync(string sessionId, CancellationToken ct = default)
    {
        return Task.FromResult<IReadOnlyList<ChatMessage>>(
            Sessions.TryGetValue(sessionId, out var list) ? list.ToList() : []);
    }
}

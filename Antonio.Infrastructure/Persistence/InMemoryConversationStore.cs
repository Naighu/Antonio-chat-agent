using System.Collections.Concurrent;
using Antonio.Core.Abstractions;
using Antonio.Core.Models;

namespace Antonio.Infrastructure.Persistence;

public class InMemoryConversationStore : IConversationStore
{
    private readonly ConcurrentDictionary<string, List<ChatMessage>> _sessions = new();

    public Task AppendAsync(string sessionId, IEnumerable<ChatMessage> messages, CancellationToken ct = default)
    {
       var list = _sessions.GetOrAdd(sessionId, _ => new List<ChatMessage>());
        lock (list)
            list.AddRange(messages);
        return Task.CompletedTask;
    }

    public Task ClearAsync(string sessionId, CancellationToken ct = default)
    {
        _sessions.TryRemove(sessionId, out _);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<ChatMessage>> GetAsync(string sessionId, CancellationToken ct = default)
    {
        if(!_sessions.TryGetValue(sessionId, out var list))
            return Task.FromResult<IReadOnlyList<ChatMessage>>([]);
    
        lock (list!)
            return Task.FromResult<IReadOnlyList<ChatMessage>>(list.ToList());   
    }
}

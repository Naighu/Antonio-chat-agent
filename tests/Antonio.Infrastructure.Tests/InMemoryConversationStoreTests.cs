using Antonio.Core.Models;
using Antonio.Infrastructure.Persistence;

namespace Antonio.Infrastructure.Tests;

public class InMemoryConversationStoreTests
{
    [Fact]
    public async Task GetAsync_UnknownSession_ReturnsEmpty()
    {
        var store = new InMemoryConversationStore();

        var history = await store.GetAsync("nope");

        Assert.Empty(history);
    }

    [Fact]
    public async Task AppendAsync_ThenGetAsync_ReturnsMessagesInOrder()
    {
        var store = new InMemoryConversationStore();

        await store.AppendAsync("s1", [new ChatMessage(Role.User, "a"), new ChatMessage(Role.Agent, "b")]);
        await store.AppendAsync("s1", [new ChatMessage(Role.User, "c")]);

        var history = await store.GetAsync("s1");
        Assert.Equal(["a", "b", "c"], history.Select(m => m.Text));
    }

    [Fact]
    public async Task ClearAsync_RemovesHistory()
    {
        var store = new InMemoryConversationStore();
        await store.AppendAsync("s1", [new ChatMessage(Role.User, "a")]);

        await store.ClearAsync("s1");

        Assert.Empty(await store.GetAsync("s1"));
    }
}
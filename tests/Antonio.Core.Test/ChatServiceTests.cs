using Antonio.Core.Models;
using Antonio.Core.Services;
using Antonio.Core.Test.Fakes;

namespace Antonio.Core.Test;


public sealed class ChatServiceTests
{
    [Fact]
    public async Task SendAsync_ReturnsTheLlmReply()
    {
        var fake = new FakeLlmClient("Hello From Test");
        var store = new FakeConversationStore();
        var service = new ChatService(fake, store);

        var reply = await service.SendAsync("session", "hi");
        Assert.Equal("Hello From Test", reply.Text);
    }
    [Fact]
    public async Task SendAsync_UserMessageToLlm()
    {
        var fake = new FakeLlmClient("ok");
        var store = new FakeConversationStore();

        var service = new ChatService(fake, store);

        await service.SendAsync("session", "turnoff the light");
        var last = fake.LastRequest!.Messages[^1];

        Assert.Equal(Role.User, last.Role);
        Assert.Equal("turnoff the light", last.Text);
    }
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task SendAsync_RejectsEmptyMessage(string message)
    {
        var store = new FakeConversationStore();

        var service = new ChatService(new FakeLlmClient("ok"), store);

        await Assert.ThrowsAsync<ArgumentException>(() => service.SendAsync("s1", message));
    }


    [Fact]
    public async Task SendAsync_IncludesPreviousTurnsInNextRequest()
    {
        var fake = new FakeLlmClient("Nice to meet you");
        var service = new ChatService(fake, new FakeConversationStore());

        await service.SendAsync("s1", "my name is Naigal");
        await service.SendAsync("s1", "what's my name?");

        var messages = fake.LastRequest!.Messages;
        Assert.Equal(3, messages.Count);
        Assert.Equal(new ChatMessage(Role.User, "my name is Naigal"), messages[0]);
        Assert.Equal(new ChatMessage(Role.Agent, "Nice to meet you"), messages[1]);
        Assert.Equal(new ChatMessage(Role.User, "what's my name?"), messages[2]);
    }

    [Fact]
    public async Task SendAsync_SavesUserAndAssistantMessages()
    {
        var store = new FakeConversationStore();
        var service = new ChatService(new FakeLlmClient("hi there"), store);

        await service.SendAsync("s1", "hello");

        Assert.Equal(2, store.Sessions["s1"].Count);
    }

    [Fact]
    public async Task SendAsync_WhenLlmFails_SavesNothing()
    {
        var store = new FakeConversationStore();
        var service = new ChatService(new ThrowingLlmClient(), store);

        await Assert.ThrowsAsync<HttpRequestException>(() => service.SendAsync("s1", "hello"));

        Assert.Empty(store.Sessions);
    }
}

using Antonio.Core.Services;
using Antonio.Core.Test.Fakes;
using Antonio.Core.Models;

namespace Antonio.Core.Test;


public sealed class ChatServiceTests
{
    [Fact]
    public async Task SendAsync_ReturnsTheLlmReply()
    {
        var fake = new FakeLlmClient("Hello From Test");
        var service = new ChatService(fake);

        var reply = await service.SendAsync("session", "hi" );
        Assert.Equal("Hello From Test", reply);
    }
    [Fact]
    public async Task SendAsync_UserMessageToLlm()
    {
        var fake =  new FakeLlmClient("ok");
        var service = new ChatService(fake);

        await service.SendAsync("session", "turnoff the light" );
        var last = fake.LastRequest!.Messages[^1];

        Assert.Equal(Role.User, last.Role);
        Assert.Equal("turnoff the light", last.Text);
    }
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task SendAsync_RejectsEmptyMessage(string message)
    {
        var service = new ChatService(new FakeLlmClient("ok"));

        await Assert.ThrowsAsync<ArgumentException>(() => service.SendAsync("s1", message));
    }
}

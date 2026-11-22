using System;
using Antonio.Core.Abstractions;
using Antonio.Core.Models;

namespace Antonio.Core.Services;

public sealed class ChatService(ILlmClient llm)
{


    public async Task<string>  SendAsync(string sessionId, string message, CancellationToken ct = default)
    {

        if(string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("Message cannot be null or whitespace.", nameof(message));
        }

        var request = new LlmRequest(
            Messages: [new ChatMessage(Role.User, message)],
            SystemPrompt: "You are Antonio, a helpful assistant."
        );

        var response = await llm.CompleteAsync(request,ct);
        return response.Text;
    }
}

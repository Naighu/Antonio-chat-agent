using Antonio.Core.Abstractions;
using Antonio.Core.Models;

namespace Antonio.Core.Services;

public sealed class ChatService(ILlmClient llm, IConversationStore store)
{

    private const string SystemPrompt = "You are Antonio, a helpful assistant.";

    public async Task<LlmResponse>  SendAsync(string sessionId, string message, CancellationToken ct = default)
    {
        
        if(string.IsNullOrWhiteSpace(sessionId))
            throw new ArgumentException("Session Id cannot be null or whitespace.", nameof(sessionId));

        if(string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message cannot be null or whitespace.", nameof(message));
        var history = await store.GetAsync(sessionId, ct);
        Console.WriteLine("Executing");

        var userMessage = new ChatMessage(Role.User, message);


        var request = new LlmRequest(
            Messages: [..history, new ChatMessage(Role.User, message)],
            SystemPrompt: SystemPrompt
        );



        var response = await llm.CompleteAsync(request,ct);

        await store.AppendAsync(sessionId, [userMessage, new ChatMessage(Role.Agent, response.Text)],ct);
        return response;
    }
}

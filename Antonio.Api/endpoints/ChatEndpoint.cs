using Antonio.Api.Dtos;
using Antonio.Core.Services;

namespace Antonio.Api.Endpoints;

public static class ChatEndpoint
{
    const string Route = "/chat";

   async public static void MapChatEndpoint(this WebApplication app)
    {
        var chatGroup = app.MapGroup(Route).WithName("Chat");

        chatGroup.MapPost("/", async (ChatRequestDto request,ChatService chat, CancellationToken ct) =>
        {
            try
            {
                var reply = await chat.SendAsync(request.SessionId, request.Message, ct);
                return Results.Ok(new ChatResponseDto(request.SessionId, reply));
                
            }catch(ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });
    }
}

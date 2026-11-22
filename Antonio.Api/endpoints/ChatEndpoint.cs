using Antonio.Api.dtos;

namespace Antonio.Api.endpoints;

public static class ChatEndpoint
{
    const string Route = "/chat";

    public static void MapChatEndpoint(this WebApplication app)
    {
        var chatGroup = app.MapGroup(Route).WithName("Chat");

        chatGroup.MapPost("/", async (ChatRequestDto request) =>
        {
            
        });
    }
}

namespace Antonio.Core.Models;


public enum Role
{
    User,
    Agent
}
public sealed record  ChatMessage(
    Role Role,
    string Text
);

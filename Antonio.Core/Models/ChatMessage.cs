namespace Antonio.Core.Models;


public enum Role
{
    user,
    agent
}
public sealed record  ChatMessage(
    Role Role,
    string Text
);

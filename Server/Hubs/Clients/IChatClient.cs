namespace Server.Hubs.Clients;

public interface IChatClient
{
    Task ReceiveMessage(string username, string message);
    Task ReceiveUsersInRoom(string[] users);
}

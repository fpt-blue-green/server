namespace Server.Hubs.Clients;

public interface IChatClient
{
    Task ReceiveMessage(string userId,string name, string message);
    Task ReceiveUsersInRoom(string[] users);
}

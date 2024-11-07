using BusinessObjects;

namespace Server.Hubs.Clients;

public interface IChatClient
{
    Task ReceiveMessage(MessageDTO message);
}

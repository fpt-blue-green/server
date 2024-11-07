using BusinessObjects;
using BusinessObjects.Models;

namespace Server
{
	public interface IGroupChatClient
	{
		Task ReceiveGroupMessage(MessageDTO campaignMessage);
		Task ReceiveUsersInRoom(Guid[] users);
	}
}

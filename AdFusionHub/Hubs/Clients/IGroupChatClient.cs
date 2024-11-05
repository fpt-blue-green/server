using BusinessObjects;
using BusinessObjects.Models;

namespace Server
{
	public interface IGroupChatClient
	{
		Task ReceiveGroupMessage(CampaignChatDTO campaignChat);
		Task ReceiveUsersInRoom(string[] users);
	}
}

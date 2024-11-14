using BusinessObjects;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using Server.Models;
using Service;

namespace Server.Hubs
{
	public class GroupChatHub : Hub<IGroupChatClient>
	{
		private readonly IDictionary<string, GroupUserConnection> _connections;
		private readonly ICampaignChatService _campaignChatService;
		private readonly IMessageService _messageService;
		private readonly ICampaignService _campaignService;
		private readonly IChatMemberService _chatMemberService;

		private readonly IUserService _userService;

        public GroupChatHub(IDictionary<string, GroupUserConnection> connections, ICampaignChatService groupChatService, IUserService userService, ICampaignService comapaignService, IMessageService messageService, IChatMemberService chatMemberService)
        {
            _connections = connections;
            _campaignChatService = groupChatService;
            _userService = userService;
            _campaignService = comapaignService;
            _messageService = messageService;
            _chatMemberService = chatMemberService;
        }

        private async Task LoadMessages(Guid userId, Guid campaignChatId)
		{
			var messages = await _messageService.GetCampaignMessagesAsync(userId, campaignChatId);

            foreach (var message in messages)
			{
				await Clients.Caller.ReceiveGroupMessage(message);
			}
		}
		private async Task<bool> IsJoinGroup(Guid userId, Guid campaignChatId)
		{
            return await _chatMemberService.CheckExistedMember(userId, campaignChatId);
        }
        public async Task JoinRoom(GroupUserConnection userConnection)
		{
			_connections[Context.ConnectionId] = userConnection;
			var isJoinGroup = await IsJoinGroup(userConnection.SenderId, userConnection.CampaignChatId);
			if (isJoinGroup)
			{
                await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.CampaignChatId.ToString());
                await LoadMessages(userConnection.SenderId, userConnection.CampaignChatId);
                await SendUsersConnected(userConnection.CampaignChatId);
			}

        }
		/*private async Task AddNewMember(Guid newUserId ,Guid campaignChatId)
		{
			if (!isMemberJoined)
			{
				var member = new ChatMemberResDTO
				{
					CampaignChatId = campaignChatId,
					UserId = newUserId,
				};
				await _chatMemberService.AddNewMember(member);
            }
		}*/
		public async Task SendMessage(string message)
		{
			if (_connections.TryGetValue(Context.ConnectionId, out var userConnection))
			{
				var senderId = userConnection.SenderId;
				var campaignChatId = userConnection.CampaignChatId;

				var sendMessage = new MessageResDTO
				{
					Content = message,
					CampaignChatId = campaignChatId,
					SenderId = senderId,
				};
				await _messageService.SaveMessageAsync(sendMessage);
				var lastMessage = await _messageService.GetLastMessage(senderId, campaignChatId);
				await Clients.Group(userConnection.CampaignChatId.ToString()).
						ReceiveGroupMessage(lastMessage);
			}
		}

		public async Task SendUsersConnected(Guid campaignChatId)
		{
			Guid[] users = _connections.Values
				.Where(c => c.CampaignChatId == campaignChatId)
				.Select(c => c.SenderId)
				.ToArray();

			await Clients.Group(campaignChatId.ToString()).ReceiveUsersInRoom(users);
		}

		public override async Task OnDisconnectedAsync(Exception? exception)
		{
			if (_connections.TryGetValue(Context.ConnectionId, out var userConnection))
			{
				_connections.Remove(Context.ConnectionId);
				await SendUsersConnected(userConnection.CampaignChatId);
			}

			await base.OnDisconnectedAsync(exception);
		}
	}
}

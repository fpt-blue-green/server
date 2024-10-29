using BusinessObjects.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using Server.Hubs.Clients;
using Server.Models;
using Service;

namespace Server.Hubs
{
	public class GroupChatHub : Hub<IChatClient>
	{
		private readonly IDictionary<string, GroupUserConnection> _connections;
		private readonly IGroupChatService _groupChatService;
		private readonly IUserService _userService;

		public GroupChatHub(IDictionary<string, GroupUserConnection> connections, IGroupChatService groupChatService, IUserService userService)
		{
			_connections = connections;
			_groupChatService = groupChatService;
			_userService = userService;
		}

		private async Task LoadMessages(Guid userId, Guid roomId)
		{
			var receiver = await _userService.GetUserById(userId);
			var messages = await _groupChatService.GetGroupMessageAsync(roomId);
			foreach (var message in messages)
			{
				await Clients.Caller.ReceiveMessage(receiver.DisplayName, message.Message);
			}
		}

		public async Task JoinRoom(GroupUserConnection userConnection)
		{
			_connections[Context.ConnectionId] = userConnection;
			await LoadMessages(Guid.Parse(userConnection.Username), Guid.Parse(userConnection.RoomId));
			await SendUsersConnected(userConnection.RoomId);
		}

		public async Task SendMessage(Guid campaignId,string message)
		{
			if (_connections.TryGetValue(Context.ConnectionId, out var userConnection))
			{
				var senderId = Guid.Parse(userConnection.Username);
				var roomId = Guid.Parse(userConnection.RoomId);

				var sender = await _userService.GetUserById(senderId);
				var roomMessage = await _groupChatService.GetGroupMessageAsync(roomId);
				if(roomMessage.IsNullOrEmpty())
				{
					//create
					var room = new CampaignChat
					{
						CampaignId = campaignId
					};
					await _groupChatService.CreateOrSaveMessageAsync(room);

				}
				else
				{
					var sendMessage = new CampaignChat
					{
						Message = message,
						CampaignId= campaignId,
						Id= roomId,
						SenderId= senderId,
						SenderName = sender.DisplayName,
						SendTime= DateTime.UtcNow,
					};
					await _groupChatService.CreateOrSaveMessageAsync(sendMessage);
					await Clients.Caller.ReceiveMessage(sender.DisplayName, message);
					await Clients.Group(userConnection.RoomId)
							.ReceiveMessage(userConnection.Username, message);
				}

				/*// Send message to the specific receiver if they're online
				var receiverConnection = _connections.Values.FirstOrDefault(c => c.Username == userConnection.Username);
				if (receiverConnection != null)
				{
					await Clients.Client(receiverConnection.Username)
						.ReceiveMessage(sender.DisplayName, message);
				}*/
			}
		}

		public async Task SendUsersConnected(string roomId)
		{
			string[] users = _connections.Values
				.Where(c => c.RoomId == roomId)
				.Select(c => c.Username)
				.ToArray();

			await Clients.Group(roomId).ReceiveUsersInRoom(users);
		}

		public override async Task OnDisconnectedAsync(Exception? exception)
		{
			if (_connections.TryGetValue(Context.ConnectionId, out var userConnection))
			{
				_connections.Remove(Context.ConnectionId);
				await SendUsersConnected(userConnection.RoomId);
			}

			await base.OnDisconnectedAsync(exception);
		}
	}
}

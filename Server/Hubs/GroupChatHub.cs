﻿using BusinessObjects.Models;
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
		private readonly ICampaignService _campaignService;

		private readonly IUserService _userService;

		public GroupChatHub(IDictionary<string, GroupUserConnection> connections, IGroupChatService groupChatService, IUserService userService, ICampaignService comapaignService)
		{
			_connections = connections;
			_groupChatService = groupChatService;
			_userService = userService;
			_campaignService = comapaignService;
		}

		private async Task LoadMessages(Guid userId, string roomId,Guid campaignId)
		{
			
			var receiver = await _userService.GetUserById(userId);
			var messages = await _groupChatService.GetGroupMessageAsync(roomId);
			if(messages.IsNullOrEmpty())
			{
				var room = new CampaignChat
				{
					RoomName = roomId,
					CampaignId = campaignId,
					Message = "Ni hao"
				};
				await _groupChatService.CreateOrSaveMessageAsync(room);
			}
			foreach (var message in messages)
			{
				await Clients.Caller.ReceiveMessage(message.SenderId?.ToString()??"", message.Sender?.DisplayName ?? "", message?.Message??"");
			}
		}

		public async Task JoinRoom(GroupUserConnection userConnection)
		{
			_connections[Context.ConnectionId] = userConnection;
			await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.RoomId);
			await LoadMessages(Guid.Parse(userConnection.Username), userConnection.RoomId,userConnection.campaignId);
			await SendUsersConnected(userConnection.RoomId);
		}

		public async Task SendMessage(string message)
		{
			if (_connections.TryGetValue(Context.ConnectionId, out var userConnection))
			{
				var senderId = Guid.Parse(userConnection.Username);
				var roomId = userConnection.RoomId;

				var sender = await _userService.GetUserById(senderId);
				var roomMessage = await _groupChatService.GetGroupMessageAsync(roomId);
				var roomName = roomMessage.Select(x => x.RoomName).FirstOrDefault();
				var campaignId = roomMessage.Select(x => x.CampaignId).FirstOrDefault();				
				var sendMessage = new CampaignChat
				{
					Message = message,
					CampaignId = campaignId,
					RoomName = roomName,
					SenderId = senderId,
					SenderName = sender.DisplayName,
					SendTime = DateTime.UtcNow,
				};
				await _groupChatService.CreateOrSaveMessageAsync(sendMessage);
				await Clients.Group(userConnection.RoomId)
						.ReceiveMessage(userConnection.Username,sender.DisplayName, message);
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

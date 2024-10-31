using Microsoft.AspNetCore.SignalR;
using Server.Hubs.Clients;
using Server.Models;
using Repositories;
using BusinessObjects.Models;
using Service;
namespace Server.Hubs;

public class ChatHub : Hub<IChatClient>
{
    private readonly IDictionary<string, UserConnection> _connections;
	private readonly IChatService _chatService;
	private readonly IUserService _userService;
    public ChatHub(IDictionary<string, UserConnection> connections, IChatService chatService, IUserService userService)
    {
        _connections = connections;
        _chatService = chatService;
        _userService = userService;
    }

    private async Task LoadMessages(Guid userId, Guid receiverId)
	{
		var messages = await _chatService.GetMessagesAsync(userId, receiverId);
		foreach (var message in messages)
		{
			await Clients.Caller.ReceiveMessage(userId.ToString(), message.SenderName, message.Message);
		}
	}

	// Bắt đầu cuộc trò chuyện và tải các tin nhắn cũ
	public async Task StartChat(UserConnection userConnection)
	{
		_connections[Context.ConnectionId] = userConnection;
		await LoadMessages(Guid.Parse(userConnection.Username), Guid.Parse(userConnection.ReceiverId));
	}

	// Gửi tin nhắn trực tiếp từ người gửi đến người nhận
	public async Task SendMessage(string message)
	{
		if (_connections.TryGetValue(Context.ConnectionId, out var userConnection))
		{
			var senderId = Guid.Parse(userConnection.Username);
			var receiverId = Guid.Parse(userConnection.ReceiverId);

			var sender = await _userService.GetUserById(senderId);
			var receiver = await _userService.GetUserById(receiverId);

			var chatRoom = new ChatRoom
			{
				Message = message,
				ReceiverId = receiverId,
				SenderId = senderId,
				ReceiverName = receiver.DisplayName,
				SenderName = sender.DisplayName,
				DateSent = DateTime.UtcNow
			};

			await _chatService.SaveMessageAsync(chatRoom);

			// Gửi tin nhắn đến người gửi (hiển thị tin nhắn đã gửi)
			await Clients.Caller.ReceiveMessage(userConnection.Username, sender.DisplayName, message);

			// Gửi tin nhắn đến người nhận nếu họ đang trực tuyến
			var receiverConnection = _connections.FirstOrDefault(c => c.Value.Username == userConnection.ReceiverId).Key;
			if (receiverConnection != null)
			{
				await Clients.Client(receiverConnection)
					.ReceiveMessage(userConnection.Username, sender.DisplayName, message);
			}
		}
	}

	public override async Task OnDisconnectedAsync(Exception? exception)
	{
		if (_connections.TryGetValue(Context.ConnectionId, out var userConnection))
		{
			_connections.Remove(Context.ConnectionId);
		}
		await base.OnDisconnectedAsync(exception);
	}
}

using BusinessObjects;
using BusinessObjects.Models;
using Microsoft.AspNetCore.SignalR;
using Server.Hubs.Clients;
using Server.Models;
using Service;
namespace Server.Hubs;

public class ChatHub : Hub<IChatClient>
{
    private readonly IDictionary<string, UserConnection> _connections;
	private readonly IMessageService _messageService;
	private readonly IUserService _userService;
    public ChatHub(IDictionary<string, UserConnection> connections, IMessageService chatService, IUserService userService)
    {
        _connections = connections;
        _messageService = chatService;
        _userService = userService;
    }

    private async Task LoadMessages(Guid userId, Guid receiverId)
	{
		var messages = await _messageService.GetMessagesAsync(userId, receiverId);
		foreach (var message in messages)
		{
			await Clients.Caller.ReceiveMessage(message);
		}
	}

	// Bắt đầu cuộc trò chuyện và tải các tin nhắn cũ
	public async Task StartChat(UserConnection userConnection)
	{
		_connections[Context.ConnectionId] = userConnection;
		await LoadMessages(userConnection.SenderId, userConnection.ReceiverId);
	}

	// Gửi tin nhắn trực tiếp từ người gửi đến người nhận
	public async Task SendMessage(string message)
	{
		if (_connections.TryGetValue(Context.ConnectionId, out var userConnection))
		{
			var senderId = userConnection.SenderId;
			var receiverId = userConnection.ReceiverId;


			var msg = new MessageResDTO
			{
				Content = message,
				ReceiverId = receiverId,
				SenderId = senderId,
			};

			await _messageService.SaveMessageAsync(msg);
            var lastMessage = await _messageService.GetLastMessage(senderId, receiverId);

            // Gửi tin nhắn đến người gửi (hiển thị tin nhắn đã gửi)
            //await Clients.Caller.ReceiveMessage(userConnection.Username, sender.DisplayName, message);

            // Gửi tin nhắn đến người nhận nếu họ đang trực tuyến
            var receiverConnection = _connections.FirstOrDefault(c => c.Value.SenderId == userConnection.ReceiverId).Key;
			if (receiverConnection != null)
			{
				await Clients.Client(receiverConnection)
					.ReceiveMessage(lastMessage);
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

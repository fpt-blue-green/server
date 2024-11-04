using Microsoft.AspNetCore.SignalR;

using Repositories;
using BusinessObjects.Models;
namespace AdFusionAPI.Hubs;

public class ChatHub : Hub<IChatClient>
{
    private readonly IDictionary<string, UserConnection> _connections;
    private readonly IUserChatRepository _userChatRepository = new UserChatRepository();
    private readonly IUserRepository _userRepository = new UserRepository();

    public ChatHub(IDictionary<string, UserConnection> connections)
    {
        _connections = connections;
    }

	private async Task LoadMessages(Guid userId, Guid receiverId)
	{
		var messages = await _userChatRepository.GetMessagesAsync(userId, receiverId);
		foreach (var message in messages)
		{
			await Clients.Caller.ReceiveMessage(message.SenderName, message.Message);
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

			var sender = await _userRepository.GetUserById(senderId);
			var receiver = await _userRepository.GetUserById(receiverId);

			var chatRoom = new UserChat
			{
				Message = message,
				ReceiverId = receiverId,
				SenderId = senderId,
				ReceiverName = receiver.DisplayName,
				SenderName = sender.DisplayName,
				DateSent = DateTime.UtcNow
			};

			await _userChatRepository.SaveMessageAsync(chatRoom);

			// Gửi tin nhắn đến người gửi (hiển thị tin nhắn đã gửi)
			await Clients.Caller.ReceiveMessage(sender.DisplayName, message);

			// Gửi tin nhắn đến người nhận nếu họ đang trực tuyến
			var receiverConnection = _connections.FirstOrDefault(c => c.Value.Username == userConnection.ReceiverId).Key;
			if (receiverConnection != null)
			{
				await Clients.Client(receiverConnection)
					.ReceiveMessage(sender.DisplayName, message);
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

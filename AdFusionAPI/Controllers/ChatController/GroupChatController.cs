using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace AdFusionAPI.Controllers.ChatController
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupChatController : Controller
    {
        private readonly IMessageService _messageService;
        private readonly ChatContactService _chatContactService;
        private readonly IChatMemberService _chatMemberService;
        private readonly ICampaignChatService _campaignChatService;
        public GroupChatController(IMessageService messageService, IChatMemberService chatMemberService, ICampaignChatService campaignChatService, ChatContactService chatContactService)
        {
            _messageService = messageService;
            _chatMemberService = chatMemberService;
            _campaignChatService = campaignChatService;
            _chatContactService = chatContactService;
        }

        [HttpPost("group/save")]
        public async Task<IActionResult> SaveMessage([FromBody] MessageResDTO messageRes)
        {
            await _messageService.SaveMessageAsync(messageRes);
            return Ok("Message saved successfully.");
        }

        [HttpGet("group/{senderId}/{receiverId}")]
        public async Task<ActionResult<List<MessageDTO>>> GetMessages(Guid senderId, Guid receiverId)
        {
            var messages = await _messageService.GetMessagesAsync(senderId, receiverId);
            return Ok(messages);
        }

        [HttpGet("group/campaign/{senderId}/{campaignChatId}")]
        public async Task<ActionResult<List<MessageDTO>>> GetCampaignMessages(Guid senderId, Guid campaignChatId)
        {
            var messages = await _messageService.GetCampaignMessagesAsync(senderId, campaignChatId);
            return Ok(messages);
        }

        [HttpGet("group/last/{senderId}/{campaignChatId}")]
        public async Task<ActionResult<MessageDTO>> GetLastMessage(Guid senderId, Guid campaignChatId)
        {
            var message = await _messageService.GetLastMessage(senderId, campaignChatId);
            return Ok(message);
        }

        [HttpGet("member/{campaignId}")]
        public async Task<ActionResult<List<ChatMemberDTO>>> GetMembersInCampaign(Guid campaignId)
        {
            var members = await _chatMemberService.GetMembersInCampaign(campaignId);
            return Ok(members);
        }

        [HttpPost("member/add")]
        public async Task<IActionResult> AddNewMember([FromBody] ChatMemberResDTO chatMemberRes)
        {
            await _chatMemberService.AddNewMember(chatMemberRes);
            return Ok("Member added successfully.");
        }

        [HttpGet("member/check/{userId}/{campaignChatId}")]
        public async Task<ActionResult<bool>> CheckExistedMember(Guid userId, Guid campaignChatId)
        {
            var exists = await _chatMemberService.CheckExistedMember(userId, campaignChatId);
            return Ok(exists);
        }

        [HttpGet("room/{campaignChatId}")]
        public async Task<ActionResult<CampaignChatDTO>> GetCampaignChatById(Guid campaignChatId)
        {
            var campaignChat = await _campaignChatService.GetCampaignChatById(campaignChatId);
            if (campaignChat == null)
            {
                return NotFound("Campaign chat not found.");
            }
            return Ok(campaignChat);
        }

        [HttpPost("room/create")]
        public async Task<IActionResult> CreateCampaignChatRoom([FromBody] CampaignChatResDTO campaignChat)
        {
            await _campaignChatService.CreateCampaignChatRoom(campaignChat);
            return Ok("Campaign chat room created successfully.");
        }
        [HttpGet("chat/contact")]
        public async Task<IActionResult> GetChatContact()
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _chatContactService.GetChatContactsAsync(user.Id);
            return Ok("Campaign chat room created successfully.");
        }
    }
}

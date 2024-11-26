using AdFusionAPI.APIConfig;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace AdFusionAPI.Controllers.ChatController
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : Controller
    {
        private readonly IMessageService _messageService;
        private readonly IChatContactService _chatContactService;
        private readonly IChatMemberService _chatMemberService;
        private readonly ICampaignChatService _campaignChatService;
        private readonly IUserService _userService;

        public ContactController(IMessageService messageService, IChatMemberService chatMemberService, ICampaignChatService campaignChatService, IChatContactService chatContactService, IUserService userService)
        {
            _messageService = messageService;
            _chatMemberService = chatMemberService;
            _campaignChatService = campaignChatService;
            _chatContactService = chatContactService;
            _userService = userService;
        }
        #region test api
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
        #endregion
        [HttpPost("campaignChat/create")]
        [BrandRequired]
        public async Task<ActionResult<CampaignChatDTO>> CreateCampaignChatRoom([FromBody] CampaignChatResDTO campaignChat)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var chatId = await _campaignChatService.CreateCampaignChatRoom(campaignChat, user);
            return Ok(chatId);
        }
        [HttpGet("chat/contacts")]
        [AuthRequired]
        public async Task<ActionResult<IEnumerable<ChatPartnerDTO>>> GetChatContact(string? searchValue)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _chatContactService.GetChatContactsAsync(user.Id, searchValue);
            return Ok(result);
        }

        [HttpGet("chat/contacts/{id}")]
        [AuthRequired]
        public async Task<ActionResult<ChatPartnerDTO>> GetChatContactById(Guid id)
        {
            var result = await _chatContactService.GetChatContactByIdAsync(id);
            return Ok(result);
        }
        [HttpGet("chat/contacts/search")]
        [AuthRequired]
        public async Task<ActionResult<List<UserDTO>>> GetChatContacts(string searchValue)
        {
            var result = await _userService.GetUserToContact(searchValue);
            return Ok(result);
        }
        [HttpPost("chat/contacts/{id}/addMembers")]
        [BrandRequired]
        public async Task<ActionResult<List<UserDTO>>> AddMembers(List<Guid> ids, [FromRoute] Guid id)
        {
            await _chatContactService.AddMemberToGroupChat(ids, id);
            return Ok();
        }
        [HttpDelete("chat/contacts/{id}/deleteMember")]
        [BrandRequired]
        public async Task<ActionResult> DeleteMembers([FromRoute] Guid id, Guid userId)
        {
            await _chatContactService.DeleteMember(userId, id);
            return Ok();
        }
    }
}

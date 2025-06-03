using Domain.Requests;
using Domain.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly GetUserChatsUseCase _getUserChatsUseCase;
        private readonly CreateGroupUseCase _createGroupUseCase;
        private readonly CreateOrGetChatUseCase _createOrGetChatUseCase;
        private readonly GetChatMessagesUseCase _getChatMessagesUseCase;
        private readonly JoinGroupUseCase _joinGroupUseCase;
        private readonly LeaveGroupUseCase _leaveGroupUseCase;
        private readonly SendMessageUseCase _sendMessageUseCase;

        public ChatController(GetUserChatsUseCase getUserChatsUseCase,
                              CreateGroupUseCase createGroupUseCase,
                              CreateOrGetChatUseCase createOrGetChatUseCase,
                              GetChatMessagesUseCase getChatMessagesUseCase,
                              JoinGroupUseCase joinGroupUseCase,
                              LeaveGroupUseCase leaveGroupUseCase,
                              SendMessageUseCase sendMessageUseCase)
        {
            _getUserChatsUseCase = getUserChatsUseCase;
            _createGroupUseCase = createGroupUseCase;
            _createOrGetChatUseCase = createOrGetChatUseCase;
            _getChatMessagesUseCase = getChatMessagesUseCase;
            _joinGroupUseCase = joinGroupUseCase;
            _leaveGroupUseCase = leaveGroupUseCase;
            _sendMessageUseCase = sendMessageUseCase;
        }

        [Authorize]
        [HttpGet("")]
        public async Task<ActionResult> GetUserChats()
        {
            var userName = User.Identity.Name;

            var result = await _getUserChatsUseCase.Handle(userName);

            if (result.Succeeded)
                return Ok(result);

            return BadRequest(result);
        }

        [Authorize]
        [HttpPost("group")]
        public async Task<ActionResult> CreateGroup(CreateGroupRequest request)
        {
            var username = User.Identity.Name;
            if (!request.MemberUserNames.Any(x => x.Equals(username)))
                request.MemberUserNames.Add(username);

            var result = await _createGroupUseCase.Handle(request);

            if (result.Succeeded)
                return Ok(result);

            return BadRequest(result);
        }

        [Authorize]
        [HttpGet("privatechat")]
        public async Task<ActionResult> GetPrivateChat(string username)
        {
            var callerUserName = User.Identity.Name;

            var result = await _createOrGetChatUseCase.Handle(callerUserName, username);

            if (result.Succeeded)
                return Ok(result);

            return BadRequest(result);
        }

        [Authorize]
        [HttpGet("messages")]
        public async Task<ActionResult> GetMessages(int chatId)
        {
            var username = User.Identity.Name;

            var result = await _getChatMessagesUseCase.Handle(username, chatId);

            if (result.Succeeded)
                return Ok(result);

            return BadRequest(result);
        }

        [Authorize]
        [HttpPost("group/join")]
        public async Task<ActionResult> JoinGroup(int groupId)
        {
            var username = User.Identity.Name;

            var result = await _joinGroupUseCase.Handle(username, groupId);

            if (result.Succeeded)
                return Ok(result);

            return BadRequest(result);
        }

        [Authorize]
        [HttpDelete("group/leave")]
        public async Task<ActionResult> LeaveGroup(int groupId)
        {
            var username = User.Identity.Name;

            var result = await _leaveGroupUseCase.Handle(username, groupId);

            if (result.Succeeded)
                return Ok(result);

            return BadRequest(result);
        }

        [Authorize]
        [HttpPost("messages")]
        public async Task<ActionResult> SendMessage(SendMessageRequest request)
        {
            var username = User.Identity.Name;
            request.SenderUserName = username;

            request.TimeStamp = DateTime.Now;
            
            var result = await _sendMessageUseCase.Handle(request);

            if (result.Succeeded)
                return Ok(result);

            return BadRequest(result);
        }
    }
}

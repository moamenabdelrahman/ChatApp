using Domain.Entities;
using Domain.Requests;
using Domain.Responses;
using Domain.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Api.SignalR
{
    [Authorize]
    public class ChatHub : Hub<IChatClient>
    {
        // {username: [connectionIds...]}
        private static Dictionary<string, List<string>> _userConnectionIds = new Dictionary<string, List<string>>();
        private static HashSet<string> _groups = new HashSet<string>();
        
        private readonly GetUserChatsUseCase _getUserChatsUseCase;
        private readonly GetChatMessagesUseCase _getChatMessagesUseCase;
        private readonly GetChatMembersUseCase _getChatMembersUseCase;
        private readonly SendMessageUseCase _sendMessageUseCase;
        private readonly GetChatPreviewUseCase _getChatPreviewUseCase;
        private readonly CreateOrGetChatUseCase _createOrGetChatUseCase;
        private readonly CreateGroupUseCase _createGroupUseCase;
        private readonly JoinGroupUseCase _joinGroupUseCase;
        private readonly GetUserUseCase _getUserUseCase;
        private readonly LeaveGroupUseCase _leaveGroupUseCase;

        public ChatHub(GetUserChatsUseCase getUserChatsUseCase,
                       GetChatMessagesUseCase getChatMessagesUseCase,
                       GetChatMembersUseCase getChatMembersUseCase,
                       SendMessageUseCase sendMessageUseCase,
                       GetChatPreviewUseCase getChatPreviewUseCase,
                       CreateOrGetChatUseCase createOrGetChatUseCase,
                       CreateGroupUseCase createGroupUseCase,
                       JoinGroupUseCase joinGroupUseCase,
                       GetUserUseCase getUserUseCase,
                       LeaveGroupUseCase leaveGroupUseCase)
        {
            _getUserChatsUseCase = getUserChatsUseCase;
            _getChatMessagesUseCase = getChatMessagesUseCase;
            _getChatMembersUseCase = getChatMembersUseCase;
            _sendMessageUseCase = sendMessageUseCase;
            _getChatPreviewUseCase = getChatPreviewUseCase;
            _createOrGetChatUseCase = createOrGetChatUseCase;
            _createGroupUseCase = createGroupUseCase;
            _joinGroupUseCase = joinGroupUseCase;
            _getUserUseCase = getUserUseCase;
            _leaveGroupUseCase = leaveGroupUseCase;
        }

        public async override Task OnConnectedAsync()
        {
            string username = Context.User.Identity.Name;
            string connectionId = Context.ConnectionId;

            if (!_userConnectionIds.ContainsKey(username))
                _userConnectionIds.Add(username, new List<string>());

            _userConnectionIds[username].Add(connectionId);

            var chats = (await _getUserChatsUseCase.Handle(username)).Data;

            foreach(var chat in chats)
            {
                await Groups.AddToGroupAsync(connectionId, $"chat-{chat.Id}");
                _groups.Add($"chat-{chat.Id}");
            }

            await Clients.Caller.RecieveChatList(chats);
        }

        public async Task<Result<List<Message>>> GetChatMessages(int chatId)
        {
            var result = await _getChatMessagesUseCase.Handle(Context.User.Identity.Name, chatId);

            return result;
        }

        public async Task<Result<List<User>>> GetChatMembers(int chatId)
        {
            var result = await _getChatMembersUseCase.Handle(Context.User.Identity.Name, chatId);

            return result;
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            string username = Context.User.Identity.Name;
            string connectionId = Context.ConnectionId;

            _userConnectionIds[username].Remove(connectionId);

            foreach(var group in _groups)
            {
                await Groups.RemoveFromGroupAsync(connectionId, group);
            }
        }

        public async Task<Result<Message>> SendMessageToChat(int chatId, string message) // validate membership, send
        {
            var request = new SendMessageRequest()
            {
                ChatId = chatId,
                Content = message,
                SenderUserName = Context.User.Identity.Name,
                TimeStamp = DateTime.Now
            };

            var result = await _sendMessageUseCase.Handle(request);
            if (!result.Succeeded)
                return result;

            var chatPreview = (await _getChatPreviewUseCase.Handle(chatId)).Data;

            await Clients.Group($"chat-{chatId}").RecieveMessage(chatId, result.Data);
            await Clients.Group($"chat-{chatId}").RecieveChatPreview(chatPreview);

            return result;
        }

        // get/create chat, check it in _groups, new? register, notify other, send
        public async Task<Result<Chat>> SendMessageToPerson(string username, string message)
        {
            var callerUserName = Context.User.Identity.Name;
            var result = await _createOrGetChatUseCase.Handle(callerUserName, username);
            if (!result.Succeeded)
                return result;

            var chat = result.Data;

            if(!_groups.Contains($"chat-{chat.Id}"))
            {
                _groups.Add($"chat-{chat.Id}");

                foreach (var connectionId in _userConnectionIds.GetValueOrDefault(callerUserName, new List<string>()))
                    await Groups.AddToGroupAsync(connectionId, $"chat-{chat.Id}");

                foreach (var connectionId in _userConnectionIds.GetValueOrDefault(username, new List<string>()))
                    await Groups.AddToGroupAsync(connectionId, $"chat-{chat.Id}");

                var newChat = (await _getChatPreviewUseCase.Handle(chat.Id)).Data;
                await Clients.Group($"chat-{chat.Id}").RecieveChatPreview(newChat);
            }

            var request = new SendMessageRequest()
            {
                ChatId = chat.Id,
                Content = message,
                SenderUserName = callerUserName,
                TimeStamp = DateTime.Now
            };

            var messageModel = (await _sendMessageUseCase.Handle(request)).Data;

            var chatPreview = (await _getChatPreviewUseCase.Handle(chat.Id)).Data;

            await Clients.Group($"chat-{chat.Id}").RecieveChatPreview(chatPreview);
            await Clients.Group($"chat-{chat.Id}").RecieveMessage(chat.Id, messageModel);

            return Result<Chat>.Ok(chatPreview);
        }

        public async Task<Result<Chat>> CreateGroup(string groupName, List<string> memberUserNames)
        {
            var callerUserName = Context.User.Identity.Name;
            if (!memberUserNames.Contains(callerUserName))
                memberUserNames.Add(callerUserName);

            var request = new CreateGroupRequest()
            {
                Name = groupName,
                MemberUserNames = memberUserNames
            };

            var result = await _createGroupUseCase.Handle(request);

            if (!result.Succeeded)
                return result;

            var newChat = result.Data;

            _groups.Add($"chat-{newChat.Id}");

            foreach(var username in memberUserNames)
            {
                foreach(var connectionId in _userConnectionIds.GetValueOrDefault(username, new List<string>()))
                {
                    await Groups.AddToGroupAsync(connectionId, $"chat-{newChat.Id}");
                }
            }

            await Clients.Group($"chat-{newChat.Id}").RecieveChatPreview(newChat);

            return result;
        }

        public async Task<Result<Chat>> JoinGroup(int groupId)
        {
            var username = Context.User.Identity.Name;
            var result = await _joinGroupUseCase.Handle(username, groupId);

            if (!result.Succeeded)
                return Result<Chat>.Fail(result.Errors);

            _groups.Add($"chat-{groupId}");
            foreach (var connectionId in _userConnectionIds.GetValueOrDefault(username, new List<string>()))
                await Groups.AddToGroupAsync(connectionId, $"chat-{groupId}");

            var chatPreview = (await _getChatPreviewUseCase.Handle(groupId)).Data;

            var user = (await _getUserUseCase.Handle(username)).Data;

            await Clients.Group($"chat-{groupId}").MemberJoined(groupId, user);
            
            return Result<Chat>.Ok(chatPreview);
        }

        public async Task<Result> LeaveGroup(int chatId)
        {
            var username = Context.User.Identity.Name;
            var result = await _leaveGroupUseCase.Handle(username, chatId);

            if (!result.Succeeded)
                return result;

            foreach (var connectionId in _userConnectionIds.GetValueOrDefault(username, new List<string>()))
                await Groups.RemoveFromGroupAsync(connectionId, $"chat-{chatId}");

            var user = (await _getUserUseCase.Handle(username)).Data;

            await Clients.Group($"chat-{chatId}").MemberLeft(chatId, user);

            return Result.Ok();
        }
    }
}

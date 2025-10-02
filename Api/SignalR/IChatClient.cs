using Api.DTOs;
using Domain.Entities;

namespace Api.SignalR
{
    public interface IChatClient
    {
        public Task RecieveChatList(List<ChatPreviewDTO> chats); // used

        public Task MemberLeft(int chatId, User member); // used

        public Task MemberJoined(int chatId, User member); // used

        public Task RecieveChatPreview(ChatPreviewDTO chat); // used

        public Task RecieveMessage(int chatId, Message message); // used
    }
}

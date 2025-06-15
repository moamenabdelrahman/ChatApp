using Domain.Entities;

namespace Api.SignalR
{
    public interface IChatClient
    {
        public Task RecieveChatList(List<Chat> chats); // used

        public Task MemberLeft(int chatId, User member); // used

        public Task MemberJoined(int chatId, User member); // used

        public Task RecieveChatPreview(Chat chat); // used

        public Task RecieveMessage(int chatId, Message message); // used
    }
}

using Domain.Entities;
using Domain.Responses;

namespace Domain.IRepositories
{
    public interface IChatRepository
    {
        public Task<List<Chat>> GetUserChatsPreview(User user);

        public Task<Chat> GetChatPreview(int chatId);

        public Task<Result<Chat>> CreateChat(Chat chat);

        public Task<Chat> GetPrivateChat(User user1, User user2);

        public Task<bool> IsInChat(User user, Chat chat);

        public Task<Chat> GetChatById(int chatId);

        public Task<List<Message>> GetChatMessages(Chat chat);

        public Task<Result> AddToGroup(User user, Chat chat);

        public Task<Result> RemoveFromGroup(User user, Chat chat);

        public Task<Result<Message>> SendMessage(Message message);

        public Task<List<User>> GetChatMembers(Chat chat);
    }
}

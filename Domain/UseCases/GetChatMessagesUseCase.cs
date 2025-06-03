using Domain.Entities;
using Domain.IRepositories;
using Domain.Responses;

namespace Domain.UseCases
{
    public class GetChatMessagesUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IChatRepository _chatRepository;

        public GetChatMessagesUseCase(IUserRepository userRepository,
                                      IChatRepository chatRepository)
        {
            _userRepository = userRepository;
            _chatRepository = chatRepository;
        }

        public async Task<Result<List<Message>>> Handle(string userName, int chatId)
        {
            var user = await _userRepository.GetUserByUserName(userName);
            var chat = await _chatRepository.GetChatById(chatId);

            if (user is null || chat is null)
                return Result<List<Message>>.Fail("Some of the provided entities aren't found!");

            if (!(await _chatRepository.IsInChat(user, chat)))
                return Result<List<Message>>.Fail("You aren't a member of this chat!");

            var messages = await _chatRepository.GetChatMessages(chat);

            return Result<List<Message>>.Ok(messages);
        }
    }
}

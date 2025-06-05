using Domain.Entities;
using Domain.IRepositories;
using Domain.Responses;

namespace Domain.UseCases
{
    public class GetChatMembersUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IChatRepository _chatRepository;

        public GetChatMembersUseCase(IUserRepository userRepository,
                                     IChatRepository chatRepository)
        {
            _userRepository = userRepository;
            _chatRepository = chatRepository;
        }

        public async Task<Result<List<User>>> Handle(string username, int chatId)
        {
            var user = await _userRepository.GetUserByUserName(username);
            var chat = await _chatRepository.GetChatById(chatId);

            if (user is null || chat is null)
                return Result<List<User>>.Fail("Some of the provided entities aren't found!");

            if (!(await _chatRepository.IsInChat(user, chat)))
                return Result<List<User>>.Fail("You aren't a member of this chat!");

            var members = await _chatRepository.GetChatMembers(chat);
            
            return Result<List<User>>.Ok(members);
        }
    }
}

using Domain.Entities;
using Domain.IRepositories;
using Domain.Responses;

namespace Domain.UseCases
{
    public class GetUserChatsUseCase
    {
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;

        public GetUserChatsUseCase(IChatRepository chatRepository,
                                   IUserRepository userRepository)
        {
            _chatRepository = chatRepository;
            _userRepository = userRepository;
        }

        public async Task<Result<List<Chat>>> Handle(string username)
        {
            var user = await _userRepository.GetUserByUserName(username);

            var chats = await _chatRepository.GetUserChatsPreview(user);

            return Result<List<Chat>>.Ok(chats);
        }
    }
}

using Domain.IRepositories;
using Domain.Responses;

namespace Domain.UseCases
{
    public class GetPrivateChatIdWithUseCase
    {
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;

        public GetPrivateChatIdWithUseCase(IChatRepository chatRepository,
                                           IUserRepository userRepository)
        {
            this._chatRepository = chatRepository;
            this._userRepository = userRepository;
        }

        public async Task<Result<int>> Handle(string selfUsername, string otherUsername)
        {
            var user1 = await _userRepository.GetUserByUserName(selfUsername);
            var user2 = await _userRepository.GetUserByUserName(otherUsername);
            if (user1 is null || user2 is null)
                return Result<int>.Fail("Some of the provided users are not found!");

            var chat = await _chatRepository.GetPrivateChat(user1, user2);
            return Result<int>.Ok(chat?.Id ?? -1);
        }
    }
}

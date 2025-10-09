using Domain.Entities;
using Domain.IRepositories;
using Domain.Responses;

namespace Domain.UseCases
{
    public class CreateOrGetChatUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IChatRepository _chatRepository;

        public CreateOrGetChatUseCase(IUserRepository userRepository,
                                      IChatRepository chatRepository)
        {
            _userRepository = userRepository;
            _chatRepository = chatRepository;
        }

        public async Task<Result<Chat>> Handle(string userName1, string userName2)
        {
            if (userName1.Equals(userName2))
                return Result<Chat>.Fail("Usernames must be different!");

            var user1 = await _userRepository.GetUserByUserName(userName1);
            var user2 = await _userRepository.GetUserByUserName(userName2);

            if (user1 is null || user2 is null)
                return Result<Chat>.Fail("Some of the provided users aren't found!");

            var chat = await _chatRepository.GetPrivateChat(user1, user2);

            if(chat is null)
            {
                var result = await _chatRepository.CreateChat(new Chat()
                {
                    Name = $"{userName1}|{userName2}",
                    Type = Enums.ChatType.Private,
                    Members = new List<User>() { user1, user2 }
                }).ConfigureAwait(false);

                chat = result.Data;
            }

            return Result<Chat>.Ok(chat);
        }
    }
}

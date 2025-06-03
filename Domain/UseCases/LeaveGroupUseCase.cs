using Domain.IRepositories;
using Domain.Responses;

namespace Domain.UseCases
{
    public class LeaveGroupUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IChatRepository _chatRepository;

        public LeaveGroupUseCase(IUserRepository userRepository,
                                 IChatRepository chatRepository)
        {
            _userRepository = userRepository;
            _chatRepository = chatRepository;
        }

        public async Task<Result> Handle(string userName, int groupId)
        {
            var user = await _userRepository.GetUserByUserName(userName);
            var group = await _chatRepository.GetChatById(groupId);

            if (user is null || group is null)
                return Result.Fail("Some of the provided entities aren't found!");

            if (!(await _chatRepository.IsInChat(user, group)))
                return Result.Fail("You are already not a member of this group!");

            if (group.Type != Enums.ChatType.Group)
                return Result.Fail("You can't leave a private chat!");

            var result = await _chatRepository.RemoveFromGroup(user, group);
            
            return result;
        }
    }
}

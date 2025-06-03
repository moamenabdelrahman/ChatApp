using Domain.Entities;
using Domain.IRepositories;
using Domain.Requests;
using Domain.RequestsValidators;
using Domain.Responses;

namespace Domain.UseCases
{
    public class CreateGroupUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IChatRepository _chatRepository;
        private readonly CreateGroupValidator _validator;

        public CreateGroupUseCase(IUserRepository userRepository,
                                  IChatRepository chatRepository,
                                  CreateGroupValidator validator)
        {
            _userRepository = userRepository;
            _chatRepository = chatRepository;
            _validator = validator;
        }

        public async Task<Result<Chat>> Handle(CreateGroupRequest request)
        {
            var validation = _validator.Validate(request);

            if (!validation.IsValid)
                return Result<Chat>.Fail(validation.Errors.Select(e => e.ErrorMessage).ToList());

            var users = new List<User>();
            foreach(var username in request.MemberUserNames)
            {
                var user = await _userRepository.GetUserByUserName(username);
                if (user is null)
                    return Result<Chat>.Fail("Some of the provided users aren't found!");
                users.Add(user);
            }

            var chat = new Chat()
            {
                Name = request.Name,
                Type = Enums.ChatType.Group,
                Members = users
            };

            var result = await _chatRepository.CreateChat(chat);

            return result;
        }
    }
}

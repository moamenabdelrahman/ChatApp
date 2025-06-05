using Domain.Entities;
using Domain.IRepositories;
using Domain.Responses;

namespace Domain.UseCases
{
    public class GetUserUseCase
    {
        private readonly IUserRepository _userRepository;

        public GetUserUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<User>> Handle(string username)
        {
            var user = await _userRepository.GetUserByUserName(username);

            if (user is null)
                return Result<User>.Fail("User isn't found!");

            return Result<User>.Ok(user);
        }
    }
}

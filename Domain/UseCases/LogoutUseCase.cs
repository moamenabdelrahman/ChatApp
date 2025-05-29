using Domain.IRepositories;
using Domain.Responses;

namespace Domain.UseCases
{
    public class LogoutUseCase
    {
        private readonly IUserRepository _userRepository;

        public LogoutUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result> Handle()
        {
            return await _userRepository.Logout();
        }
    }
}

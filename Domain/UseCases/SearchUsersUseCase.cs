using Domain.Entities;
using Domain.IRepositories;

namespace Domain.UseCases
{
    public class SearchUsersUseCase
    {
        private readonly IUserRepository _userRepository;

        public SearchUsersUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<User>> Handle(string text)
        {
            var users = await _userRepository.Search(text);
            
            return users;
        }
    }
}

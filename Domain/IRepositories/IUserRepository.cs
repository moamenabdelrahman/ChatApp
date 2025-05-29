using Domain.Entities;
using Domain.Requests;
using Domain.Responses;

namespace Domain.IRepositories
{
    public interface IUserRepository
    {
        public Task<Result<User>> Create(RegisterRequest request);

        public Task<Result> Login(LoginRequest request);

        public Task<Result> Logout();
    }
}

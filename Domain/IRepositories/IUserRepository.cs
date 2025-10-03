using Domain.DTOs;
using Domain.Entities;
using Domain.Requests;
using Domain.Responses;

namespace Domain.IRepositories
{
    public interface IUserRepository
    {
        public Task<User> GetUserByUserName(string userName);

        public Task<Result<User>> Create(RegisterRequest request);

        public Task<Result<LoginDTO>> Login(LoginRequest request);

        public Task<Result> Logout();

        public Task<List<User>> Search(string text);

        public Task<Result> ResetPassword(User user, string token, string newPassword);
    }
}

using Domain.Entities;
using Domain.IRepositories;
using Domain.Requests;
using Domain.Responses;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public UserRepository(UserManager<AppUser> userManager,
                              SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<Result<User>> Create(RegisterRequest request)
        {
            var appUser = new AppUser()
            {
                UserName = request.UserName,
                Email = request.Email
            };
            var result = await _userManager.CreateAsync(appUser, request.Password);

            if(result.Succeeded)
            {
                var user = new User() { Id = appUser.Id, UserName = appUser.UserName, Email = appUser.Email };
                return Result<User>.Ok(user);
            }

            return Result<User>.Fail(result.Errors.Select(e => e.Description).ToList());
        }

        public async Task<Result> Login(LoginRequest request)
        {
            var result = await _signInManager.PasswordSignInAsync(request.UserName, request.Password, request.RememberMe, false);

            return result.Succeeded? Result.Ok() : Result.Fail();
        }

        public async Task<Result> Logout()
        {
            await _signInManager.SignOutAsync();
            return Result.Ok();
        }
    }
}

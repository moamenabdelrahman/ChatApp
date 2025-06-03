using AutoMapper;
using Domain.Entities;
using Domain.IRepositories;
using Domain.Requests;
using Domain.Responses;
using Infrastructure.Data;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IMapper _mapper;

        public UserRepository(UserManager<AppUser> userManager,
                              SignInManager<AppUser> signInManager,
                              IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }

        public async Task<Result<User>> Create(RegisterRequest request)
        {
            var appUser = _mapper.Map<AppUser>(request);

            var result = await _userManager.CreateAsync(appUser, request.Password);

            if(result.Succeeded)
            {
                var user = _mapper.Map<User>(appUser);
                return Result<User>.Ok(user);
            }

            return Result<User>.Fail(result.Errors.Select(e => e.Description).ToList());
        }

        public async Task<User> GetUserByUserName(string userName)
        {
            var appUser = await _userManager.FindByNameAsync(userName);

            return _mapper.Map<User>(appUser);
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

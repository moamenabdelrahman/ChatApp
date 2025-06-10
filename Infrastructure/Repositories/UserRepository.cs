using AutoMapper;
using Domain.Entities;
using Domain.IRepositories;
using Domain.Requests;
using Domain.Responses;
using Infrastructure.Data;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;

        public UserRepository(UserManager<AppUser> userManager,
                              SignInManager<AppUser> signInManager,
                              AppDbContext appDbContext,
                              IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appDbContext = appDbContext;
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

            return result.Succeeded ?
                Result.Ok() :
                Result.Fail(result.IsNotAllowed ? "Email isn't confirmed!" : "Wrong Credentials!");
        }

        public async Task<Result> Logout()
        {
            await _signInManager.SignOutAsync();
            return Result.Ok();
        }

        public async Task<Result> ResetPassword(User user, string token, string newPassword)
        {
            var appUser = await _userManager.FindByNameAsync(user.UserName);

            var result = await _userManager.ResetPasswordAsync(appUser, token, newPassword);

            if (result.Succeeded)
                return Result.Ok();

            return Result.Fail(result.Errors.Select(x => x.Description).ToList());
        }

        public async Task<List<User>> Search(string text)
        {
            text = text.ToUpper();
            var userEntities = await _appDbContext.Users
                                    .Where(u => u.NormalizedUserName.Contains(text))
                                    .ToListAsync().ConfigureAwait(false);

            return _mapper.Map<List<User>>(userEntities);
        }
    }
}

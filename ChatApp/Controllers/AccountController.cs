using Domain.Requests;
using Domain.Responses;
using Domain.UseCases;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Security.Claims;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AccountController : ControllerBase
    {
        private readonly RegisterUseCase _registerUseCase;
        private readonly LoginUseCase _loginUseCase;
        private readonly LogoutUseCase _logoutUseCase;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public AccountController(RegisterUseCase registerUseCase,
                                 LoginUseCase loginUseCase,
                                 LogoutUseCase logoutUseCase,
                                 SignInManager<AppUser> signInManager,
                                 UserManager<AppUser> userManager)
        {
            _registerUseCase = registerUseCase;
            _loginUseCase = loginUseCase;
            _logoutUseCase = logoutUseCase;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<ActionResult> Register(RegisterRequest request)
        {
            var result = await _registerUseCase.Handle(request);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginRequest request)
        {
            var result = await _loginUseCase.Handle(request);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        [HttpGet]
        public async Task<ActionResult> ExternalLogin(string provider, string successReturnUrl, string failureReturnUrl)
        {
            var providers = (await _signInManager.GetExternalAuthenticationSchemesAsync())
                                .Select(x => x.Name).ToList();

            if (!providers.Contains(provider))
                return String.IsNullOrEmpty(failureReturnUrl) ? Content("Invalid Provider!") : Redirect(failureReturnUrl);

            var callBackUrl = Url.Action(action: "ExternalLoginCallBack",
                                         controller: "Account",
                                         values: new { successReturnUrl, failureReturnUrl });

            var props = _signInManager.ConfigureExternalAuthenticationProperties(provider, callBackUrl);

            return new ChallengeResult(provider, props);
        }

        [HttpGet]
        public async Task<ActionResult> ExternalLoginCallBack(string successReturnUrl, string failureReturnUrl)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();

            if (info is null)
                return String.IsNullOrEmpty(failureReturnUrl) ? Content("Login Failed!") : Redirect(failureReturnUrl);

            var fname = info.Principal.FindFirstValue(ClaimTypes.GivenName);
            var lname = info.Principal.FindFirstValue(ClaimTypes.Surname);
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider,
                                                                       info.ProviderKey, false).ConfigureAwait(false);

            if (result.Succeeded)
                return String.IsNullOrEmpty(successReturnUrl) ? Content("Login Succeeded!") : Redirect(successReturnUrl);

            var appUser = await _userManager.FindByEmailAsync(email);

            if(appUser is null)
            {
                appUser = new AppUser() { UserName = email, Email = email };
                await _userManager.CreateAsync(appUser);
            }

            var loginInfo = new UserLoginInfo(info.LoginProvider, info.ProviderKey, info.ProviderDisplayName);
            await _userManager.AddLoginAsync(appUser, loginInfo);

            await _signInManager.SignInAsync(appUser, false);

            return String.IsNullOrEmpty(successReturnUrl) ? Content("Login Succeeded!") : Redirect(successReturnUrl);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Logout()
        {
            var result = await _logoutUseCase.Handle();
            return Ok(result);
        }
    }
}

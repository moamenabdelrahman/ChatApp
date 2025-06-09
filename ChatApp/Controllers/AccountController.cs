using Domain.Requests;
using Domain.UseCases;
using Infrastructure;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
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
        private readonly EmailService _emailService;

        public AccountController(RegisterUseCase registerUseCase,
                                 LoginUseCase loginUseCase,
                                 LogoutUseCase logoutUseCase,
                                 SignInManager<AppUser> signInManager,
                                 UserManager<AppUser> userManager,
                                 EmailService emailService)
        {
            _registerUseCase = registerUseCase;
            _loginUseCase = loginUseCase;
            _logoutUseCase = logoutUseCase;
            _signInManager = signInManager;
            _userManager = userManager;
            _emailService = emailService;
        }

        [HttpPost]
        public async Task<ActionResult> Register(RegisterRequest request, string loginUrl)
        {
            var result = await _registerUseCase.Handle(request);

            if (!result.Succeeded)
                return BadRequest(result);

            var appUser = await _userManager.FindByNameAsync(result.Data.UserName);

            await SendEmailConfirmation(appUser, loginUrl);

            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult> ConfirmEmail(string UserName, string Token, string loginUrl)
        {
            var appUser = await _userManager.FindByNameAsync(UserName);

            if (appUser is null)
                return Content("Confirmation Failed!");

            var result = await _userManager.ConfirmEmailAsync(appUser, Token);

            if (!result.Succeeded)
                return Content("Confirmation Failed!");

            return String.IsNullOrEmpty(loginUrl) ?
                Content("Confirmation Succeeded, you can login now!") :
                Redirect(loginUrl);
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

            var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
            var x = await _userManager.ConfirmEmailAsync(appUser, confirmationToken);

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

        private async Task SendEmailConfirmation(AppUser appUser, string loginUrl)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
            
            var url = Url.Action(action: "ConfirmEmail",
                                 controller: "Account",
                                 protocol: HttpContext.Request.Scheme,
                                 values: new { UserName = appUser.UserName, Token = token, LoginUrl = loginUrl });

            await _emailService.SendEmailAsync(appUser.Email,
                                               "ChatApp - Email Confirmation",
                                               $"Click here to confirm: {url}").ConfigureAwait(false);
        }
    }
}

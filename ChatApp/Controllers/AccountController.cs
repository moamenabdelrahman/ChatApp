using Domain.Requests;
using Domain.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AccountController : ControllerBase
    {
        private readonly RegisterUseCase _registerUseCase;
        private readonly LoginUseCase _loginUseCase;
        private readonly LogoutUseCase _logoutUseCase;

        public AccountController(RegisterUseCase registerUseCase,
                                 LoginUseCase loginUseCase,
                                 LogoutUseCase logoutUseCase)
        {
            _registerUseCase = registerUseCase;
            _loginUseCase = loginUseCase;
            _logoutUseCase = logoutUseCase;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Register(RegisterRequest request)
        {
            var result = await _registerUseCase.Handle(request);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginRequest request)
        {
            var result = await _loginUseCase.Handle(request);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        [Authorize]
        public async Task<ActionResult> Logout()
        {
            var result = await _logoutUseCase.Handle();
            return Ok(result);
        }
    }
}

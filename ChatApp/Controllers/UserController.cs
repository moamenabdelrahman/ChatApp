using Domain.Entities;
using Domain.Responses;
using Domain.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly SearchUsersUseCase _searchUsersUseCase;

        public UserController(SearchUsersUseCase searchUsersUseCase)
        {
            _searchUsersUseCase = searchUsersUseCase;
        }

        [HttpGet("search")]
        public async Task<ActionResult> Search(string searchTerm)
        {
            searchTerm = searchTerm ?? "";

            var users = await _searchUsersUseCase.Handle(searchTerm);

            var result = Result<List<User>>.Ok(users);
            
            return Ok(result);
        }
    }
}

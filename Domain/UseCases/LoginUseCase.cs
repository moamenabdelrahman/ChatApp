using Domain.IRepositories;
using Domain.Requests;
using Domain.RequestsValidators;
using Domain.Responses;

namespace Domain.UseCases
{
    public class LoginUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly LoginValidator _validator;

        public LoginUseCase(IUserRepository userRepository, LoginValidator validator)
        {
            _userRepository = userRepository;
            _validator = validator;
        }

        public async Task<Result<string>> Handle(LoginRequest request)
        {
            var result = await _validator.ValidateAsync(request);

            if (!result.IsValid)
                return Result<string>.Fail(result.Errors.Select(x => x.ErrorMessage).ToList());

            return await _userRepository.Login(request);
        }
    }
}

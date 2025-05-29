using Domain.Entities;
using Domain.IRepositories;
using Domain.Requests;
using Domain.RequestsValidators;
using Domain.Responses;

namespace Domain.UseCases
{
    public class RegisterUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly RegisterValidator _validator;

        public RegisterUseCase(IUserRepository userRepository, RegisterValidator validator)
        {
            _userRepository = userRepository;
            _validator = validator;
        }

        public async Task<Result<User>> Handle(RegisterRequest request)
        {
            var result = await _validator.ValidateAsync(request);

            if (!result.IsValid)
                return Result<User>.Fail(result.Errors.Select(x => x.ErrorMessage).ToList());

            return await _userRepository.Create(request);
        }
    }
}

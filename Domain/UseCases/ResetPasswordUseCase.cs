using Domain.IRepositories;
using Domain.Requests;
using Domain.RequestsValidators;
using Domain.Responses;

namespace Domain.UseCases
{
    public class ResetPasswordUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly ResetPasswordValidator _validator;

        public ResetPasswordUseCase(IUserRepository userRepository,
                                    ResetPasswordValidator validator)
        {
            _userRepository = userRepository;
            _validator = validator;
        }

        public async Task<Result> Handle(ResetPasswordRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);

            if (!validationResult.IsValid)
                return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage).ToList());

            var user = await _userRepository.GetUserByUserName(request.UserName);

            if (user is null)
                return Result.Fail("User isn't found!");

            var result = await _userRepository.ResetPassword(user, request.Token, request.NewPassword);

            return result;
        }
    }
}

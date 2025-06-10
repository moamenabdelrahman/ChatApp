using Domain.Requests;
using FluentValidation;

namespace Domain.RequestsValidators
{
    public class ResetPasswordValidator : AbstractValidator<ResetPasswordRequest>
    {
        public ResetPasswordValidator()
        {
            RuleFor(x => x.UserName).NotEmpty();

            RuleFor(x => x.NewPassword).NotEmpty();

            RuleFor(x => x.ConfirmPassword).Equal(x => x.NewPassword);

            RuleFor(x => x.Token).NotEmpty();
        }
    }
}

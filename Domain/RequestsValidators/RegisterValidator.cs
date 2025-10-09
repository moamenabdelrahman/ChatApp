using Domain.Requests;
using FluentValidation;

namespace Domain.RequestsValidators
{
    public class RegisterValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.UserName).NotEmpty()
                                    .Must(username => !username.Contains("|"))
                                    .WithMessage("Username cannot contain the '|' character.");
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty();
            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("Passwords don't match!");
        }
    }
}

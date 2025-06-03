using Domain.Requests;
using FluentValidation;

namespace Domain.RequestsValidators
{
    public class SendMessageValidator : AbstractValidator<SendMessageRequest>
    {
        public SendMessageValidator()
        {
            RuleFor(x => x.SenderUserName).NotEmpty();

            RuleFor(x => x.Content).NotEmpty();
        }
    }
}

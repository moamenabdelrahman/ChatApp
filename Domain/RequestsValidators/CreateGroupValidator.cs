using Domain.Requests;
using FluentValidation;

namespace Domain.RequestsValidators
{
    public class CreateGroupValidator : AbstractValidator<CreateGroupRequest>
    {
        public CreateGroupValidator()
        {
            RuleFor(x => x.Name).NotEmpty();

            RuleFor(x => x.MemberUserNames)
                .Must(lst => lst.ToHashSet().Count >= 2);
        }
    }
}

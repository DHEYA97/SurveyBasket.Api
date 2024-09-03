using SurveyBasket.Api.Abstractions.Consts;

namespace SurveyBasket.Api.Contract.Auth.User;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .Length(3,100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .Length(3, 100);

        RuleFor(x => x.Email)
           .NotEmpty()
           .EmailAddress();
        RuleFor(x => x.Roles)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.Roles)
            .Must(x => x.Distinct().Count() == x.Count())
                .WithMessage("You Cannot duplicate Rule")
                .When(x => x is not null);
    }
}
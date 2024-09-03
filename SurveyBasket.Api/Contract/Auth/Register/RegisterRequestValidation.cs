using SurveyBasket.Api.Abstractions.Consts;

namespace SurveyBasket.Api.Contract.Auth.Register
{
    public class RegisterRequestValidation : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidation()
        {
            RuleFor(l => l.Email)
                   .NotEmpty()
                   .EmailAddress();

            RuleFor(l => l.Password)
                .NotEmpty()
                .Matches(RegexPatterns.Password)
                .WithMessage("Password should be at least 8 digits and should contains Lowercase, NonAlphanumeric and Uppercase");

            RuleFor(l => l.FirstName)
                .NotEmpty();

            RuleFor(l => l.LastName)
                .NotEmpty();
        }
    }

}

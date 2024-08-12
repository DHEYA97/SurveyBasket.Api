using SurveyBasket.Api.Contract.Auth;

namespace SurveyBasket.Api.Contract.Validation
{
    public class LoginRequestValidation : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidation() 
        {
            RuleFor(l => l.Email)
                   .NotEmpty();
                   //.EmailAddress();
        }
    }
}

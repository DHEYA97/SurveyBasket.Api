using SurveyBasket.Api.Contract.Auth;

namespace SurveyBasket.Api.Contract.Validation
{
    public class RefreshTokenRequestValidation : AbstractValidator<RefreshTokenRequest>
    {
        public RefreshTokenRequestValidation() 
        {
            RuleFor(r => r.Token)
                   .NotEmpty();
            RuleFor(r => r.RefreshToken)
                   .NotEmpty();
        }
    }
}

using SurveyBasket.Api.Abstractions.Consts;
using SurveyBasket.Api.Contract.ConfirmEmail;

namespace SurveyBasket.Api.Contract.ResetPassword
{
    public class ResetPasswordRequestValidation : AbstractValidator<ResetPasswordRequest>
    {
        public ResetPasswordRequestValidation()
        {
            RuleFor(l => l.Email)
                .NotEmpty()
                .EmailAddress();
            
            RuleFor(l => l.Code)
                .NotEmpty();
            RuleFor(l => l.NewPassword)
                .NotEmpty()
                .Matches(RegexPatterns.Password)
                .WithMessage("Password should be at least 8 digits and should contains Lowercase, NonAlphanumeric and Uppercase");
        }
    }

}

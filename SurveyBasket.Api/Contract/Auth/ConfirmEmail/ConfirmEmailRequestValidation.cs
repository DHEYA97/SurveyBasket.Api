using SurveyBasket.Api.Abstractions.Consts;
using SurveyBasket.Api.Contract.ConfirmEmail;

namespace SurveyBasket.Api.Contract.Register
{
    public class ConfirmEmailRequestValidation : AbstractValidator<ConfirmEmailRequest>
    {
        public ConfirmEmailRequestValidation()
        {
            RuleFor(l => l.UserId)
                .NotEmpty();
            
            RuleFor(l => l.Code)
                .NotEmpty();
        }
    }

}

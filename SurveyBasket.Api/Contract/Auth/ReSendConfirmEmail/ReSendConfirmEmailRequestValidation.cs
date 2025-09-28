namespace SurveyBasket.Api.Contract.ReSendConfirmEmail
{
    public class ReSendConfirmEmailRequestValidation : AbstractValidator<ReSendConfirmEmailRequest>
    {
        public ReSendConfirmEmailRequestValidation()
        {

            RuleFor(l => l.Email)
                .NotEmpty();
        }
    }

}

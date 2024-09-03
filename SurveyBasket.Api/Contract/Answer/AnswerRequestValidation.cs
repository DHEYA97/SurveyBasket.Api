using SurveyBasket.Api.Contract.Answer;

namespace SurveyBasket.Api.Contract.Question
{
    public class AnswerRequestValidation : AbstractValidator<AnswerRequest>
    {
        public AnswerRequestValidation()
        {
            RuleFor(q => q.Content)
                   .NotEmpty()
                   .Length(3, 1000);

        }

        
    }
}

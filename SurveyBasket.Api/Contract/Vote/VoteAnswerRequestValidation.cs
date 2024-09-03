using SurveyBasket.Api.Contract.Vote;

namespace SurveyBasket.Api.Contract.Question
{
    public class VoteAnswerRequestValidation : AbstractValidator<VotesAnswerRequest>
    {
        public VoteAnswerRequestValidation()
        {
            RuleFor(v => v.QuestionId)
                   .GreaterThan(0);

            RuleFor(v => v.AnswerId)
                    .GreaterThan(0);
        }
    }
}

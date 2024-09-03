using SurveyBasket.Api.Contract.Vote;

namespace SurveyBasket.Api.Contract.Question
{
    public class VoteRequestValidation : AbstractValidator<VotesRequest>
    {
        public VoteRequestValidation()
        {
            RuleFor(v => v.Answers)
                   .NotEmpty();
            RuleForEach(v => v.Answers)
                .SetInheritanceValidator(a => a.Add(new VoteAnswerRequestValidation()));
        }
    }
}

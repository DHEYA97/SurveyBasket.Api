namespace SurveyBasket.Api.Contract.Question
{
    public class QuestionValidation : AbstractValidator<QuestionRequest>
    {
        public QuestionValidation()
        {
            RuleFor(q => q.Content)
                   .NotEmpty()
                   .Length(3, 1000);

            RuleFor(q => q.Answers)
                .NotNull();


            RuleFor(q=>q.Answers)
                .Must(q=>q.Count() > 1)
                .WithMessage("Question should has at lest 2 Answers")
                .When(q=>q.Answers != null);


            RuleFor(q => q.Answers)
                .Must(q=>q.Distinct().Count() == q.Count())
                .WithMessage(p => "You Cannot duplicate Answer for the same question")
                .When(q => q.Answers != null); ;

        }

        
    }
}

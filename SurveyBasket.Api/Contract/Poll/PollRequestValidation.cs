namespace SurveyBasket.Api.Contract.Poll
{
    public class PollRequestValidation : AbstractValidator<PollRequest>
    {
        public PollRequestValidation()
        {
            RuleFor(p => p.Title)
                   .NotEmpty()
                   .Length(3, 100);

            RuleFor(p => p.Summary)
                   .NotEmpty()
                   .Length(3, 1500);

            RuleFor(p => p.StartAt)
                .NotEmpty()
                .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
                .WithMessage("{PropertyName} must be >= " + $"{DateOnly.FromDateTime(DateTime.Today)}");

            RuleFor(p => p)
                .Must(HasValidDate)
                .WithName(nameof(PollRequest.EndAt))
                .WithMessage(p => "{PropertyName} must be >= " + $"{p.StartAt}");

        }

        private bool HasValidDate(PollRequest pollRequest)
        {
            return pollRequest.EndAt >= pollRequest.StartAt;
        }
    }
}

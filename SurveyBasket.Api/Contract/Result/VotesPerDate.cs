namespace SurveyBasket.Api.Contract.Result
{
    public record VotesPerDate(
        DateOnly Date,
        int CountOfVote
        );
}

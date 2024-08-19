namespace SurveyBasket.Api.Contract.Result
{
    public record PollsAnswerCountResponse(
        string Name,
        int countOfVote
    );
}

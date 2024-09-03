namespace SurveyBasket.Api.Contract.Result
{
    public record VoteByPollResponse(
        string Name,
        DateTime VoteDate,
        IEnumerable<SelectedAnswersResponse> SelectedAnswers
        );
}

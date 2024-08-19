namespace SurveyBasket.Api.Contract.Result
{
    public record PollsQuestionAnswerCountResponse(
        string Name,
        IEnumerable<PollsAnswerCountResponse> SelectedAnswer
    );
}

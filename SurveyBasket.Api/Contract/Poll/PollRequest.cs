namespace SurveyBasket.Api.Contract.Poll
{
    public record PollRequest(
         string Title,
         string Summary,
         DateOnly StartAt,
         DateOnly EndAt
        );
}

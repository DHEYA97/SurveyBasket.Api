namespace SurveyBasket.Api.Contract.Poll
{
    public record PollRequest(
         string Title,
         string Summary,
         bool IsPublished,
         DateOnly StartAt,
         DateOnly EndAt
        );
}

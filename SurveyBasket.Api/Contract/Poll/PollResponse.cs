namespace SurveyBasket.Api.Contract.Poll
{
    public record PollResponse(
         int Id,
         string Title,
         string Summary,
         bool IsPublished,
         DateOnly StartAt,
         DateOnly EndAt
        );
}

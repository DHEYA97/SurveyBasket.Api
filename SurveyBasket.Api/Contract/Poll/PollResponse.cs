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

    public record PollResponseV2(
         int Id,
         string Title,
         string Summary,
         DateOnly StartAt,
         DateOnly EndAt
        );
}

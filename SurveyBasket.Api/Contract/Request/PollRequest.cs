
namespace SurveyBasket.Api.Contract.Request
{
    public record PollRequest(
         string Title,
         string Summary,
         bool IsPublished,
         DateOnly StartAt,
         DateOnly EndAt
        );  
}

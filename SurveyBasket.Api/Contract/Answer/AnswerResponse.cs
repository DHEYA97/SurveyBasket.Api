namespace SurveyBasket.Api.Contract.Question
{
    public record AnswerResponse(
        int Id,
        string Content,
        bool IsActive
        );
}

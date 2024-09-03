namespace SurveyBasket.Api.Contract.Question
{
    public record QuestionResponse(
         int Id,
        string Content,
        IEnumerable<AnswerResponse> Answers
        );
}

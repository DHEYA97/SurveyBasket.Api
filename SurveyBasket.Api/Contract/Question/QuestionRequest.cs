namespace SurveyBasket.Api.Contract.Question
{
    public record QuestionRequest(
         string Content,
         List<string> Answers
        );
}

namespace SurveyBasket.Api.Contract.Result
{
    public record SelectedAnswersResponse(
        string QuestionName,
        string AnswerName
        );
    
}

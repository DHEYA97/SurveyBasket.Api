namespace SurveyBasket.Api.Contract.Vote
{
    public record VotesAnswerRequest(
         int QuestionId,
         int AnswerId
        );
}
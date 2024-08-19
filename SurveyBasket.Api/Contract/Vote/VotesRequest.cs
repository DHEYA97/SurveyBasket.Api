namespace SurveyBasket.Api.Contract.Vote
{
    public record VotesRequest(
         IEnumerable<VotesAnswerRequest> Answers
        );
}
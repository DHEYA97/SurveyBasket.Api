namespace SurveyBasket.Api.Contract.Result
{
    public record PollsVoteResponse(
        string Name,
        IEnumerable<VoteByPollResponse> Votes
    );
}

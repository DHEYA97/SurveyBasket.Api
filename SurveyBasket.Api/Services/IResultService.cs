using SurveyBasket.Api.Contract.Result;

namespace SurveyBasket.Api.Services
{
    public interface IResultService
    {
        Task<Result<PollsVoteResponse>> GetResultByPollIdAsync(int pollId,CancellationToken cancellationToken);
        Task<Result<IEnumerable<VotesPerDate>>> GetResultPerDayAsync(int pollId, CancellationToken cancellationToken);
        Task<Result<IEnumerable<PollsQuestionAnswerCountResponse>>> GetQuestionCountAsync(int pollId, CancellationToken cancellationToken);
    }
}

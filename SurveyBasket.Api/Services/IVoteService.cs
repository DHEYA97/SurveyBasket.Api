using SurveyBasket.Api.Contract.Question;
using SurveyBasket.Api.Contract.Vote;

namespace SurveyBasket.Api.Services
{
    public interface IVoteService
    {
        Task<Result> AddAsync(int pollId, string userId, VotesRequest request, CancellationToken cancellationToken = default);
    }
}

using SurveyBasket.Api.Contract.Poll;

namespace SurveyBasket.Api.Services
{
    public interface IPollService
    {
        Task<Result<PollResponse>> GetAsync(int id, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<PollResponse>>> GetAllCurrentAsync(CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<PollResponse>>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Result<PollResponse>> AddAsync(PollRequest pollRequest, CancellationToken cancellationToken = default);
        Task<Result> UpdateAsync(int id,PollRequest pollRequest, CancellationToken cancellationToken = default);
        Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<Result> TogglePublishAsync(int id, CancellationToken cancellationToken = default);
    }
}

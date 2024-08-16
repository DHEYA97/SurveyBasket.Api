using SurveyBasket.Api.Contract.Poll;
using SurveyBasket.Api.Contract.Question;

namespace SurveyBasket.Api.Services
{
    public interface IQuestionService
    {
        Task<Result<QuestionResponse>> GetAsync(int pollId, int id, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int pollId,CancellationToken cancellationToken = default);
        Task<Result<QuestionResponse>> AddAsync(int pollId ,QuestionRequest questionRequest, CancellationToken cancellationToken = default);
        Task<Result> UpdateAsync(int pollId, int id, QuestionRequest questionRequest, CancellationToken cancellationToken = default);
        Task<Result> ToggleStatusAsync(int pollId, int id, CancellationToken cancellationToken = default);
    }
}

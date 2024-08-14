using Azure.Core;
using SurveyBasket.Api.Contract.Poll;
using SurveyBasket.Api.Entities;
using SurveyBasket.Api.Persistence;

namespace SurveyBasket.Api.Services
{
    public class PollService(ApplicationDbContext context) : IPollService
    {
        private readonly ApplicationDbContext _context = context;
        public async Task<Result<PollResponse>> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            var pollResponse = await _context.Polls.FindAsync(id, cancellationToken);
            if (pollResponse is null)
                return Result.Failure<PollResponse>(PollErrors.PollNotFound);
            return Result.Success(pollResponse.Adapt<PollResponse>());
        }
        public async Task<Result<IEnumerable<PollResponse>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var pollsResponse = await _context.Polls.AsNoTracking().ToListAsync(cancellationToken);
            return Result.Success(pollsResponse.Adapt<IEnumerable<PollResponse>>());
        }
        public async Task<PollResponse> AddAsync(PollRequest pollRequest, CancellationToken cancellationToken = default)
        {
            var poll = pollRequest.Adapt<Poll>();
            await _context.AddAsync(poll, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return poll.Adapt<PollResponse>();
        }
        
        public async Task<Result> UpdateAsync(int id, PollRequest pollRequest, CancellationToken cancellationToken = default)
        {
            var upPoll = await _context.Polls.FindAsync(id, cancellationToken);
            if (upPoll is null) 
                return Result.Failure(PollErrors.PollNotFound); 
            upPoll.Title = pollRequest.Title;
            upPoll.Summary = pollRequest.Summary;
            upPoll.StartAt = pollRequest.StartAt;
            upPoll.EndAt = pollRequest.EndAt;
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var delPoll = await _context.Polls.FindAsync(id, cancellationToken);
            if (delPoll is null)
                return Result.Failure(PollErrors.PollNotFound);
            _context.Polls.Remove(delPoll);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        public async Task<Result> TogglePublishAsync(int id, CancellationToken cancellationToken = default)
        {
            var poll = await _context.Polls.FindAsync(id, cancellationToken);
            if (poll is null)
                return Result.Failure(PollErrors.PollNotFound);
            poll.IsPublished = !poll.IsPublished;
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}

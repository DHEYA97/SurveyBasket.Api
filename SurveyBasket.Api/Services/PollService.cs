using Azure.Core;
using Hangfire;
using SurveyBasket.Api.Contract.Poll;
using SurveyBasket.Api.Entities;
using SurveyBasket.Api.Persistence;
using System.Collections.Generic;

namespace SurveyBasket.Api.Services
{
    public class PollService(
        ApplicationDbContext context,
        INotificationService notificationService
        ) : IPollService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly INotificationService _notificationService = notificationService;

        public async Task<Result<PollResponse>> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            var pollResponse = await _context.Polls.FindAsync(id, cancellationToken);
            if (pollResponse is null)
                return Result.Failure<PollResponse>(PollErrors.PollNotFound);
            return Result.Success(pollResponse.Adapt<PollResponse>());
        }
        public async Task<Result<IEnumerable<PollResponse>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var pollsResponse = await _context.Polls
                                              .AsNoTracking()
                                              .ProjectToType<PollResponse>()
                                              .ToListAsync(cancellationToken);
            return Result.Success<IEnumerable<PollResponse>>(pollsResponse);
        }
        public async Task<Result<IEnumerable<PollResponse>>> GetAllCurrentAsync(CancellationToken cancellationToken = default)
        {
            var pollsResponse = await _context.Polls
                                              .Where(p=>p.IsPublished && p.StartAt <= DateOnly.FromDateTime(DateTime.UtcNow) && p.EndAt >= DateOnly.FromDateTime(DateTime.UtcNow))                              
                                              .AsNoTracking()
                                              .ProjectToType<PollResponse>()
                                              .ToListAsync(cancellationToken);
            return Result.Success<IEnumerable<PollResponse>>(pollsResponse);
        }
        public async Task<Result<PollResponse>> AddAsync(PollRequest pollRequest, CancellationToken cancellationToken = default)
        {
            var isExistTitle = _context.Polls.Any(x=>x.Title == pollRequest.Title);
            if(isExistTitle)
               return Result.Failure<PollResponse>(PollErrors.DuplicatePollTitle);

            var poll = pollRequest.Adapt<Poll>();
            await _context.AddAsync(poll, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success(poll.Adapt<PollResponse>());
        }
        
        public async Task<Result> UpdateAsync(int id, PollRequest pollRequest, CancellationToken cancellationToken = default)
        {
            var upPoll = await _context.Polls.FindAsync(id, cancellationToken);
            if (upPoll is null) 
                return Result.Failure(PollErrors.PollNotFound);
            
            var isExistTitle = _context.Polls.Any(x => x.Title == pollRequest.Title && x.Id != id);
            if (isExistTitle)
                return Result.Failure<PollResponse>(PollErrors.DuplicatePollTitle);

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
            if(poll.IsPublished && poll.StartAt == DateOnly.FromDateTime(DateTime.UtcNow))
                BackgroundJob.Enqueue(() => _notificationService.SendEmailInBackgroundJob(null));
            return Result.Success();
        }
    }
}

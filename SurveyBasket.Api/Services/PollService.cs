

using SurveyBasket.Api.Entities;
using SurveyBasket.Api.Persistence;

namespace SurveyBasket.Api.Services
{
    public class PollService(ApplicationDbContext context) : IPollService
    {
        private readonly ApplicationDbContext _context = context;
        public async Task<Poll?> GetAsync(int id, CancellationToken cancellationToken = default) => await _context.Polls.FindAsync(id,cancellationToken);
        public async Task<IEnumerable<Poll>> GetAllAsync(CancellationToken cancellationToken = default) => await _context.Polls.AsNoTracking().ToListAsync(cancellationToken);
        public async Task<Poll> AddAsync(Poll poll, CancellationToken cancellationToken = default)
        {
            await _context.Polls.AddAsync(poll,cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return poll;
        }

        public async Task<bool> UpdateAsync(int id, Poll poll, CancellationToken cancellationToken = default)
        {
            var upPoll = await GetAsync(id,cancellationToken);
            if (upPoll is null) 
                return false;
            upPoll.Title = poll.Title;
            upPoll.Summary = poll.Summary;
            upPoll.StartAt = poll.StartAt;
            upPoll.EndAt = poll.EndAt;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var delPoll = await GetAsync(id,cancellationToken);
            if (delPoll is null)
                return false;
            _context.Polls.Remove(delPoll);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        public async Task<bool> TogglePublishAsync(int id, CancellationToken cancellationToken = default)
        {
            var poll = await GetAsync(id, cancellationToken);
            if (poll is null)
                return false;
            poll.IsPublished = !poll.IsPublished;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}

using SurveyBasket.Api.Contract.Vote;
using SurveyBasket.Api.Persistence;

namespace SurveyBasket.Api.Services
{
    public class VoteService(ApplicationDbContext context) : IVoteService
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<Result> AddAsync(int pollId, string userId, VotesRequest request, CancellationToken cancellationToken = default)
        {
            var hasVote = await _context.Votes.AnyAsync(v => v.PollId == pollId && v.UserId == userId, cancellationToken);
            if (hasVote)
                return Result.Failure(VoteErrors.DuplicatedVote);
            var isExistPoll = await _context.Polls.AnyAsync(p => p.Id == pollId && p.IsPublished && p.StartAt <= DateOnly.FromDateTime(DateTime.UtcNow) && p.EndAt >= DateOnly.FromDateTime(DateTime.UtcNow));
            if (isExistPoll)
                return Result.Failure(PollErrors.PollNotFound);

            var availableQuestion = await _context.Questions
                                                  .Where(q => q.PollId == pollId && q.IsActive)
                                                  .Select(q => q.Id)
                                                  .ToListAsync();
            if(!request.Answers.Select(r=>r.QuestionId).SequenceEqual(availableQuestion))
                return Result.Failure(VoteErrors.InvalidQuestions);

            //Todo check answer Id

            Vote vote = new()
            {
                PollId = pollId,
                UserId = userId,
                VoteAnswers = request.Answers.Adapt<IEnumerable<VoteAnswer>>().ToList()
            };
            await _context.Votes.AddAsync(vote,cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}

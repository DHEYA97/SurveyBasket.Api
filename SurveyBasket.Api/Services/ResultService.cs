using SurveyBasket.Api.Contract.Poll;
using SurveyBasket.Api.Contract.Result;
using SurveyBasket.Api.Persistence;
using System.Collections.Generic;
using System.Linq;

namespace SurveyBasket.Api.Services
{
    public class ResultService(ApplicationDbContext context) : IResultService
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<Result<PollsVoteResponse>> GetResultByPollIdAsync(int pollId, CancellationToken cancellationToken)
        {
            var pollResponse = await _context.Polls.FindAsync(pollId, cancellationToken);
            if (pollResponse is null)
                return Result.Failure<PollsVoteResponse>(PollErrors.PollNotFound);
            var votesInPoll = await _context.Polls
                                            .Where(v => v.Id == pollId)
                                            .Select(v => new PollsVoteResponse(
                                                    v.Title,
                                                    v.Votes.Select(v=> new VoteByPollResponse(
                                                           $"{v.User.FirstName} {v.User.LastName}",
                                                                v.SubmittedOn,
                                                                v.VoteAnswers.Select(a=>new SelectedAnswersResponse(
                                                                    a.Question.Content,
                                                                    a.Answer.Content
                                                                ))
                                                        )
                                                ))).SingleOrDefaultAsync(cancellationToken);
            if(votesInPoll is null)
                return Result.Failure<PollsVoteResponse>(VoteErrors.VoteNotFound);
            return Result.Success(votesInPoll);
        }
        public async Task<Result<IEnumerable<VotesPerDate>>> GetResultPerDayAsync(int pollId, CancellationToken cancellationToken)
        {
            var pollResponse = await _context.Polls.FindAsync(pollId, cancellationToken);
            if (pollResponse is null)
                return Result.Failure< IEnumerable <VotesPerDate>> (PollErrors.PollNotFound);

            var votesBerDay = await _context.Votes
                                            .Where(v => v.Poll.Id == pollId)
                                            .GroupBy(v=> new {Date = DateOnly.FromDateTime(v.SubmittedOn)})
                                            .Select(v=> new VotesPerDate(
                                                v.Key.Date,
                                                v.Count()
                                                )).ToListAsync(cancellationToken);
            if (votesBerDay is null)
                return Result.Failure<IEnumerable<VotesPerDate>> (VoteErrors.VoteNotFound);
            return Result.Success<IEnumerable<VotesPerDate>>(votesBerDay);
        }
        public async Task<Result<IEnumerable<PollsQuestionAnswerCountResponse>>> GetQuestionCountAsync(int pollId, CancellationToken cancellationToken)
        {
            var pollResponse = await _context.Polls.FindAsync(pollId, cancellationToken);
            if (pollResponse is null)
                return Result.Failure<IEnumerable<PollsQuestionAnswerCountResponse>>(PollErrors.PollNotFound);

            var votesBerDay = await _context.VoteAnswers
                                            .Where(v => v.Question.PollId == pollId)
                                            .Select(a => new PollsQuestionAnswerCountResponse(
                                                     a.Question.Content,
                                                     a.Question.VoteAnswers.GroupBy(g => new { g.AnswerId, AnswerName = g.Answer.Content })
                                                                 .Select(g => new PollsAnswerCountResponse(
                                                                            g.Key.AnswerName,
                                                                            g.Count()
                                                                     )))
                                                ).ToListAsync(cancellationToken);
            if (votesBerDay is null)
                return Result.Failure<IEnumerable<PollsQuestionAnswerCountResponse>>(VoteErrors.VoteNotFound);
            return Result.Success<IEnumerable<PollsQuestionAnswerCountResponse>>(votesBerDay);
        }
    }
}

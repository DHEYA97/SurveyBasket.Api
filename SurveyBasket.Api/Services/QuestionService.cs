
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.Memory;
using SurveyBasket.Api.Contract.Common;
using SurveyBasket.Api.Contract.Poll;
using SurveyBasket.Api.Contract.Question;
using SurveyBasket.Api.Persistence;
using System.Linq.Dynamic.Core;

namespace SurveyBasket.Api.Services
{
    public class QuestionService(ApplicationDbContext context,
        ////Add OutputCache to cancel
        //IOutputCacheStore outputCache
        ////Add InmemoryCache
        //IMemoryCache memoryCache
        //ICacheService cacheService
        HybridCache cache
        ) : IQuestionService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly HybridCache _cache = cache;
        
        //private readonly ICacheService _cacheService = cacheService;
        //private readonly IOutputCacheStore _outputCache = outputCache;
        //private readonly IMemoryCache _memoryCache = memoryCache;
        private const string cachePrefix = "availableQuestion";
        public async Task<Result<QuestionResponse>> GetAsync(int pollId, int id, CancellationToken cancellationToken = default)
        {
            var isPollExist = await _context.Polls.FindAsync(pollId, cancellationToken);
            if (isPollExist is null)
                return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);

            var question = await _context.Questions
                                        .Where(q => q.PollId == pollId && q.Id == id && q.IsActive)
                                        .Include(q => q.Answers)
                                        .Select(q => new QuestionResponse
                                        (
                                            q.Id,
                                            q.Content,
                                            q.Answers.Where(a => a.IsActive).Select(a => new AnswerResponse
                                            (
                                                a.Id,
                                                a.Content,
                                                a.IsActive
                                            ))
                                        )).AsNoTracking()
                                          .SingleOrDefaultAsync(cancellationToken);
            if (question is null)
                return Result.Failure<QuestionResponse>(QuestionErrors.QuestionNotFound);
            return Result.Success(question);

        }
        public async Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int pollId, CancellationToken cancellationToken = default)
        {
            var cacheKey = $"{cachePrefix}{pollId}"; 

            var isPollExist = await _context.Polls.FindAsync(pollId, cancellationToken);
            if (isPollExist is null)
                return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

            var questions = await _cache.GetOrCreateAsync<IEnumerable<QuestionResponse>>(
                                                cacheKey,
                                                async cacheEntry =>
                                                {
                                                    return await _context.Questions
                                                          .Where(q => q.PollId == pollId && q.IsActive)
                                                          .Include(q => q.Answers)
                                                          .Select(q => new QuestionResponse
                                                          (
                                                              q.Id,
                                                              q.Content,
                                                              q.Answers.Where(a => a.IsActive).Select(a => new AnswerResponse
                                                              (
                                                                  a.Id,
                                                                  a.Content,
                                                                  a.IsActive
                                                              ))
                                                          ))
                                                          .AsNoTracking()
                                                          .ToListAsync(cancellationToken);
                                                });
            #region Distriputed Cache
            //var questions = await _cacheService.GetAsync<IEnumerable<QuestionResponse>>(cacheKey, cancellationToken);

            //if(questions is null)
            //{
            //    questions =  await _context.Questions
            //                                .Where(q => q.PollId == pollId && q.IsActive)
            //                                .Include(q => q.Answers)
            //                                .Select(q => new QuestionResponse
            //                                (
            //                                    q.Id,
            //                                    q.Content,
            //                                    q.Answers.Where(a => a.IsActive).Select(a => new AnswerResponse
            //                                    (
            //                                        a.Id,
            //                                        a.Content,
            //                                        a.IsActive
            //                                    ))
            //                                ))
            //                                .AsNoTracking()
            //                                .ToListAsync(cancellationToken);

            //    await _cacheService.SetAsync(cacheKey,questions, cancellationToken);
            //} 
            #endregion


            if (questions!.Any())
                return Result.Success<IEnumerable<QuestionResponse>>(questions!);
            
            return Result.Failure<IEnumerable<QuestionResponse>>(QuestionErrors.QuestionNotFound);

        }
        public async Task<Result<PageList<QuestionResponse>>> GetAllWithPaginationAsync(int pollId, FilterResponse filter,CancellationToken cancellationToken = default)
        {

            var isPollExist = await _context.Polls.FindAsync(pollId, cancellationToken);
            if (isPollExist is null)
                return Result.Failure<PageList<QuestionResponse>>(PollErrors.PollNotFound);

            var query = _context.Questions
                                .Where(q => q.PollId == pollId && q.IsActive);
            if (!string.IsNullOrEmpty(filter.SearchValue))
            {
                query = query.Where(x => x.Content.Contains(filter.SearchValue));
            }
            if (!string.IsNullOrEmpty(filter.SortColumn))
            {
                query = query.OrderBy($"{filter.SortColumn} {filter.SortDirection}");
            }
           var  source = query.Include(q => q.Answers)
                                .Select(q => new QuestionResponse
                                (
                                    q.Id,
                                    q.Content,
                                    q.Answers.Where(a => a.IsActive).Select(a => new AnswerResponse
                                    (
                                        a.Id,
                                        a.Content,
                                        a.IsActive
                                    ))
                                ))
                                .AsNoTracking();

            var questions = await PageList<QuestionResponse>.CreateAsync(source, filter.PageNumber, filter.PageSize, cancellationToken);

            if (questions is not null)
                return Result.Success(questions);

            return Result.Failure<PageList<QuestionResponse>>(QuestionErrors.QuestionNotFound);

        }
        public async Task<Result<IEnumerable<QuestionResponse>>> GetAvailableAsync(int pollId, string userId, CancellationToken cancellationToken = default)
        {
            var hasVote = await _context.Votes.AnyAsync(v => v.PollId == pollId && v.UserId == userId,cancellationToken);
            if (hasVote) 
                return Result.Failure<IEnumerable<QuestionResponse>>(VoteErrors.DuplicatedVote);
            var isExistPoll = await _context.Polls.AnyAsync(p => p.Id == pollId && p.IsPublished && p.StartAt <= DateOnly.FromDateTime(DateTime.UtcNow) && p.EndAt >= DateOnly.FromDateTime(DateTime.UtcNow));
            if(!isExistPoll)
                return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);
            var questions = await _context.Questions
                                          .Where(q => q.PollId == pollId && q.IsActive)
                                          .Include(q => q.Answers)
                                          .Select(q => new QuestionResponse(
                                                                    q.Id,
                                                                    q.Content,
                                                                    q.Answers.Where(a=>a.IsActive)
                                                                     .Select(a=>new AnswerResponse(a.Id,a.Content,a.IsActive))
                                                                     ))
                                          .AsNoTracking()
                                          .ToListAsync(cancellationToken);

            return Result.Success<IEnumerable<QuestionResponse>>(questions);
        }
        public async Task<Result<QuestionResponse>> AddAsync(int pollId, QuestionRequest questionRequest, CancellationToken cancellationToken = default)
        {
            var isPollExist = await _context.Polls.FindAsync(pollId, cancellationToken);
            if (isPollExist is null)
                return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);

            var questionIsExists = await _context.Questions.AnyAsync(x => x.Content == questionRequest.Content && x.PollId == pollId, cancellationToken: cancellationToken);

            if (questionIsExists)
                return Result.Failure<QuestionResponse>(QuestionErrors.DuplicateQuestionContent);

            var question = questionRequest.Adapt<Question>();
            question.PollId = pollId;

            //Add answer to question With out Mapster
            //questionRequest.Answers.ForEach(answer => { question.Answers.Add(new Answer { Content = answer }); });

            await _context.AddAsync(question);
            await _context.SaveChangesAsync(cancellationToken);
            ////OutPut cache
            //await _outputCache.EvictByTagAsync("CacheTag", cancellationToken);
            //_memoryCache.Remove($"{cachePrefix}{pollId}");
            //await _cacheService.RemoveAsync($"{cachePrefix}{pollId}", cancellationToken);
            await _cache.RemoveAsync($"{cachePrefix}{pollId}", cancellationToken);
            return Result.Success(question.Adapt<QuestionResponse>());
        }

        public async Task<Result> UpdateAsync(int pollId, int id, QuestionRequest questionRequest, CancellationToken cancellationToken = default)
        {
            var isPollExist = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);
            if (!isPollExist)
                return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);

            var question = await _context.Questions
                                          .Where(q => q.PollId == pollId && q.Id == id)
                                          .Include(q => q.Answers)
                                          .SingleOrDefaultAsync(cancellationToken);
            if (question is null)
                return Result.Failure<QuestionResponse>(QuestionErrors.QuestionNotFound);

            var questionIsExist = await _context.Questions
                                          .AnyAsync(q => q.PollId == pollId &&
                                                    q.Id != id &&
                                                    q.Content == questionRequest.Content,
                                                    cancellationToken);
            if (questionIsExist)
                return Result.Failure<QuestionResponse>(QuestionErrors.DuplicateQuestionContent);

            question.Content = questionRequest.Content;
            var questionList = question.Answers.ToList();
            questionList.ForEach(answer =>
            {
                if (questionRequest.Answers.Contains(answer.Content))
                    answer.IsActive = true;
                answer.IsActive = false;
            });


            var currentAnswers = questionList.Select(a => a.Content).ToList();
            var newAnswer = questionRequest.Answers.Except(currentAnswers).ToList();
            newAnswer.ForEach(answer =>
                question.Answers.Add(new Answer { Content = answer })
            );
            await _context.SaveChangesAsync(cancellationToken);
            //await _outputCache.EvictByTagAsync("CacheTag", cancellationToken);
            //_memoryCache.Remove($"{cachePrefix}{pollId}");
            //await _cacheService.RemoveAsync($"{cachePrefix}{pollId}", cancellationToken);
            await _cache.RemoveAsync($"{cachePrefix}{pollId}", cancellationToken);
            return Result.Success();
        }

        public async Task<Result> ToggleStatusAsync(int pollId, int id, CancellationToken cancellationToken = default)
        {
            var questions = await _context.Questions.SingleOrDefaultAsync(q => q.PollId == pollId && q.Id == id, cancellationToken);
            if (questions is null)
                return Result.Failure(QuestionErrors.QuestionNotFound);
            questions.IsActive = !questions.IsActive;
            await _context.SaveChangesAsync(cancellationToken);
            //await _outputCache.EvictByTagAsync("CacheTag", cancellationToken);
            //_memoryCache.Remove($"{cachePrefix}{pollId}");
            //await _cacheService.RemoveAsync($"{cachePrefix}{pollId}", cancellationToken);
            await _cache.RemoveAsync($"{cachePrefix}{pollId}", cancellationToken);
            return Result.Success();
        }
    }
}

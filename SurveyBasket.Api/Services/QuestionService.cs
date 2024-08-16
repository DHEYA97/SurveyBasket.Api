using Azure.Core;
using SurveyBasket.Api.Contract.Poll;
using SurveyBasket.Api.Contract.Question;
using SurveyBasket.Api.Controllers;
using SurveyBasket.Api.Entities;
using SurveyBasket.Api.Persistence;
using System.Collections.Generic;

namespace SurveyBasket.Api.Services
{
    public class QuestionService(ApplicationDbContext context) : IQuestionService
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<Result<QuestionResponse>> GetAsync(int pollId, int id, CancellationToken cancellationToken = default)
        {
            var isPollExist = await _context.Polls.FindAsync(pollId, cancellationToken);
            if (isPollExist is null)
                return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);

            var question = await _context.Questions
                                          .Where(q => q.PollId == pollId && q.Id == id)
                                          .Include(q => q.Answers.Where(a => a.IsActive))
                                          .AsNoTracking()
                                          .SingleOrDefaultAsync(cancellationToken);
            if (question is null)
                return Result.Failure<QuestionResponse>(QuestionErrors.QuestionNotFound);

            QuestionResponse response = new QuestionResponse
            (question.Id,
                  question.Content,
                  question.Answers.Select(a => new AnswerResponse
                                              (
                                                  a.Id,
                                                  a.Content,
                                                  a.IsActive
                                              ))
            );

            return Result.Success(response);

        }
        public async Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int pollId, CancellationToken cancellationToken = default)
        {
            var isPollExist = await _context.Polls.FindAsync(pollId, cancellationToken);
            if (isPollExist is null)
                return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

            var questions = await _context.Questions
                                          .Where(q => q.PollId == pollId)
                                          .Include(q => q.Answers.Where(a=>a.IsActive))
                                          //.Where(q => q.IsActive == true)
                                          //.Select(q => new QuestionResponse
                                          //(
                                          //    q.Id,
                                          //    q.Content,
                                          //    q.Answers.Select(a => new AnswerResponse
                                          //    (
                                          //        a.Id,
                                          //        a.Content,
                                          //        a.IsActive
                                          //    ))
                                          //))

                                          //.ProjectToType<QuestionResponse>()
                                          .AsNoTracking()
                                          .ToListAsync(cancellationToken);
            if (questions.Any())
            {
                List<QuestionResponse> responses = [];
                foreach (var question in questions)
                {
                    responses.Add(new QuestionResponse(
                        question.Id,
                        question.Content,
                        question.Answers.Select(a => new AnswerResponse
                                                   (
                                                       a.Id,
                                                       a.Content,
                                                       a.IsActive
                                                   ))
                        )
                    );
                }
                return Result.Success<IEnumerable<QuestionResponse>>(responses);
            }
            return Result.Failure<IEnumerable<QuestionResponse>>(QuestionErrors.QuestionNotFound);

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
            return Result.Success();
        }

        public async Task<Result> ToggleStatusAsync(int pollId, int id, CancellationToken cancellationToken = default)
        {
            var questions = await _context.Questions.SingleOrDefaultAsync(q => q.PollId == pollId && q.Id == id, cancellationToken);
            if (questions is null)
                return Result.Failure(QuestionErrors.QuestionNotFound);
            questions.IsActive = !questions.IsActive;
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}

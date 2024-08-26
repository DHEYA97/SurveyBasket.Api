using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OutputCaching;
using SurveyBasket.Api.Contract.Vote;
using SurveyBasket.Api.Extensions;

namespace SurveyBasket.Api.Controllers
{
    [Route("api/polls/{pollId}/vote")]
    [ApiController]
    //[Authorize]
    public class VotesController(IQuestionService questionService,IVoteService voteService) : ControllerBase
    {
        private readonly IQuestionService _questionService = questionService;
        private readonly IVoteService _voteService = voteService;

        [HttpGet("")]
        //[ResponseCache(Duration =60)]
        [OutputCache(PolicyName = "CachePolicy")]
        public async Task<IActionResult> Start([FromRoute] int pollId,CancellationToken cancellationToken)
        {
            var userId = "21abd155-4cc7-4ddd-a7c6-2253e6b2ade7";//User.GetUserId()!;
            var result = await _questionService.GetAvailableAsync(pollId, userId, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        [HttpPost("")]
        public async Task<IActionResult> Vote([FromRoute] int pollId,[FromBody] VotesRequest request, CancellationToken cancellationToken)
        {
            var result = await _voteService.AddAsync(pollId,User.GetUserId()!,request,cancellationToken);
            return result.IsSuccess ? Created() : result.ToProblem();
        }
    }
}
namespace SurveyBasket.Api.Controllers
{
    [Route("api/polls/{pollId}/result")]
    [ApiController]
    [HasPermission(Permissions.Results)]
    public class ResultsController(IResultService resultService) : ControllerBase
    {
        private readonly IResultService _resultService = resultService;
        [HttpGet]
        public async Task<IActionResult> GetPollVotes([FromRoute] int pollId, CancellationToken cancellationToken)
        {
            var pollVotes = await _resultService.GetResultByPollIdAsync(pollId, cancellationToken);
            return pollVotes.IsSuccess ? Ok(pollVotes.Value) : pollVotes.ToProblem();
        }
        [HttpGet("per-day")]
        public async Task<IActionResult> GetPollVotesPerday([FromRoute] int pollId, CancellationToken cancellationToken)
        {
            var pollVotes = await _resultService.GetResultPerDayAsync(pollId, cancellationToken);
            return pollVotes.IsSuccess ? Ok(pollVotes.Value) : pollVotes.ToProblem();
        }
        [HttpGet("per-question")]
        public async Task<IActionResult> GetVotesPerQuestion([FromRoute] int pollId, CancellationToken cancellationToken)
        {
            var pollVotes = await _resultService.GetQuestionCountAsync(pollId, cancellationToken);
            return pollVotes.IsSuccess ? Ok(pollVotes.Value) : pollVotes.ToProblem();
        }
    }
}

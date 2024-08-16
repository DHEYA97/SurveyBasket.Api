
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using SurveyBasket.Api.Contract.Poll;

namespace SurveyBasket.Api.Controllers

{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    
    //[DisableCors]
   // [EnableCors("MyPolicy")]
    public class Polls(IPollService pollService) : ControllerBase
    {
        private readonly IPollService _pollService = pollService;

        [HttpGet("")]
        public async Task<IActionResult> GetAll()
        {
            var pollsResult = await _pollService.GetAllAsync();
            return  Ok(pollsResult.Value);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute]int id)
        {
            #region local Mapster configration
            //var config = new TypeAdapterConfig();
            //config.NewConfig<Poll, PollResponse>()
            //       .Map(des => des.Note, src => src.Description);


            //var poll = _pollService.Get(id);
            //var pollDto = poll.Adapt<PollResponse>(config);
            #endregion

            var pollResult = await _pollService.GetAsync(id);
            return pollResult.IsSuccess ? Ok(pollResult.Value) : Problem(statusCode: StatusCodes.Status404NotFound, title: pollResult.Error.Code, detail: pollResult.Error.Description);
        }

        
        [HttpPost("")]
        public async Task<IActionResult> Add([FromBody] PollRequest request,
        CancellationToken cancellationToken)
        {
            var newPollResult = await _pollService.AddAsync(request, cancellationToken);

            return newPollResult.IsSuccess 
                ? CreatedAtAction(nameof(Get), new { id = newPollResult.Value.Id }, newPollResult.Value) 
                : newPollResult.ToProblem();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest pollRequest, CancellationToken cancellationToken)
        {
            var isUpdatedResult = await _pollService.UpdateAsync(id, pollRequest, cancellationToken);
            return isUpdatedResult.IsSuccess ? NoContent() : isUpdatedResult.ToProblem();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            var isDeleteResult = await _pollService.DeleteAsync(id,cancellationToken);
            return isDeleteResult.IsSuccess ? NoContent() : isDeleteResult.ToProblem();
        }
        [HttpPut("{id}/togglePublish")]
        public async Task<IActionResult> TogglePublish([FromRoute] int id, CancellationToken cancellationToken)
        {
            var isToggledResult = await _pollService.TogglePublishAsync(id,cancellationToken);
            return isToggledResult.IsSuccess ? NoContent() : isToggledResult.ToProblem();
        }
        [HttpPost("Test")]
        public IActionResult Test([FromBody] PollRequest poll)
        {
            return Ok(poll);
        }
    }
}
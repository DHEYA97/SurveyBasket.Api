
using Microsoft.AspNetCore.Authorization;
using SurveyBasket.Api.Contract.Poll;

namespace SurveyBasket.Api.Controllers

{
    [Route("api/[controller]")]
    [ApiController]
    public class Polls(IPollService pollService) : ControllerBase
    {
        private readonly IPollService _pollService = pollService;

        [HttpGet("")]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var polls = await _pollService.GetAllAsync();
            var pollsDto = polls.Adapt<IEnumerable<PollResponse>>();
            return Ok(pollsDto);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            #region local Mapster configration
            //var config = new TypeAdapterConfig();
            //config.NewConfig<Poll, PollResponse>()
            //       .Map(des => des.Note, src => src.Description);


            //var poll = _pollService.Get(id);
            //var pollDto = poll.Adapt<PollResponse>(config);
            #endregion

            var poll = await _pollService.GetAsync(id);
            var pollDto = poll.Adapt<PollResponse>();
            return poll is null ? NotFound() : Ok(pollDto);
        }

        [HttpPost("")]
        public async Task<IActionResult> Add(PollRequest poll,CancellationToken cancellationToken)
        {
            var addPoll = await _pollService.AddAsync(poll.Adapt<Poll>(),cancellationToken);
            return CreatedAtAction(nameof(Add), new { id = addPoll.Id }, addPoll);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, PollRequest poll, CancellationToken cancellationToken)
        {
            var isUpdated = await _pollService.UpdateAsync(id, poll.Adapt<Poll>(),cancellationToken);
            if (!isUpdated)
                return NotFound();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var isDeleted = await _pollService.DeleteAsync(id,cancellationToken);
            if (!isDeleted)
                return NotFound();
            return NoContent();
        }
        [HttpPut("{id}/togglePublish")]
        public async Task<IActionResult> TogglePublish(int id, CancellationToken cancellationToken)
        {
            var isToggled = await _pollService.TogglePublishAsync(id,cancellationToken);
            if (!isToggled)
                return NotFound();
            return NoContent();
        }
        [HttpPost("Test")]
        public IActionResult Test([FromBody] PollRequest poll)
        {
            return Ok(poll);
        }
    }
}
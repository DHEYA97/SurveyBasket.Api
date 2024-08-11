
using SurveyBasket.Api.Contract.Request;

namespace SurveyBasket.Api.Controllers

{
    [Route("api/[controller]")]
    [ApiController]
    public class Polls(IPollService pollService) : ControllerBase
    {
        private readonly IPollService _pollService = pollService;

        [HttpGet("")]
        public IActionResult GetAll()
        {

            var polls = _pollService.GetAll();
            var pollsDto = polls.Adapt<IEnumerable<PollResponse>>();
            return Ok(pollsDto);
        }
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            #region local Mapster configration
            //var config = new TypeAdapterConfig();
            //config.NewConfig<Poll, PollResponse>()
            //       .Map(des => des.Note, src => src.Description);


            //var poll = _pollService.Get(id);
            //var pollDto = poll.Adapt<PollResponse>(config);
            #endregion

            var poll = _pollService.Get(id);
            var pollDto = poll.Adapt<PollResponse>();
            return poll is null ? NotFound() : Ok(pollDto);
        }

        [HttpPost("")]
        public IActionResult Add(CreatedPollRequest poll)
        {
            var addPoll = _pollService.Add(poll.Adapt<Poll>());
            return CreatedAtAction(nameof(Add), new { id = addPoll.Id }, addPoll);
        }
        [HttpPut("{id}")]
        public IActionResult Update(int id, CreatedPollRequest poll)
        {
            var isUpdated = _pollService.Update(id, poll.Adapt<Poll>());
            if (!isUpdated)
                return NotFound();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var isDeleted = _pollService.Delete(id);
            if (!isDeleted)
                return NotFound();
            return NoContent();
        }
        [HttpPost("Test")]
        public IActionResult Test([FromBody] CreatedPollRequest poll)
        {
            return Ok(poll);
        }
    }
}
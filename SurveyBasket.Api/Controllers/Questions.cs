
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Contract.Poll;
using SurveyBasket.Api.Contract.Question;

namespace SurveyBasket.Api.Controllers

{
    [Route("api/polls/{pollId}/[controller]")]
    [ApiController]
    [Authorize]
    
    public class Questions(IQuestionService questionService) : ControllerBase
    {
        private readonly IQuestionService _questionService = questionService;

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int pollId, [FromRoute] int id, CancellationToken cancellationToken)
        {
            var questionsResult = await _questionService.GetAsync(pollId, id, cancellationToken);

            return questionsResult.IsSuccess ? Ok(questionsResult.Value) : questionsResult.ToProblem();
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAll([FromRoute] int pollId,CancellationToken cancellationToken)
        {
            var questionsResult = await _questionService.GetAllAsync(pollId, cancellationToken);

            return questionsResult.IsSuccess ? Ok(questionsResult.Value) :questionsResult.ToProblem() ;
        }

        [HttpPost("")]
        public async Task<IActionResult> Add([FromRoute] int pollId, [FromBody] QuestionRequest request,
        CancellationToken cancellationToken = default)
        {
            var newQuestionResult = await _questionService.AddAsync(pollId,request, cancellationToken);

            return newQuestionResult.IsSuccess ? CreatedAtAction(nameof(Get), new { pollId, newQuestionResult.Value.Id }, newQuestionResult.Value)
            : newQuestionResult.ToProblem();

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int pollId,[FromRoute] int id, [FromBody] QuestionRequest questionRequest, CancellationToken cancellationToken)
        {
            var isUpdatedResult = await _questionService.UpdateAsync(pollId,id, questionRequest, cancellationToken);
            return isUpdatedResult.IsSuccess 
                   ? NoContent() 
                   : isUpdatedResult.ToProblem();
        }

        [HttpPut("{id}/toggleStatus")]
        public async Task<IActionResult> ToggleStatus([FromRoute] int pollId, [FromRoute] int id, CancellationToken cancellationToken)
        {
            var isToggledResult = await _questionService.ToggleStatusAsync(pollId,id, cancellationToken);
            return isToggledResult.IsSuccess ? NoContent() : isToggledResult.ToProblem();
        }
        
    }
}
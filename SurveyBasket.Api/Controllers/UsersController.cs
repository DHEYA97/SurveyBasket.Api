using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Api.Contract.Auth.User;

namespace SurveyBasket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;
        [HttpGet("")]
        [HasPermission(Permissions.GetUsers)]
        public async Task<IActionResult> GetAllUser(CancellationToken cancellationToken)
        {
            return Ok(await _userService.GetAllUserAsync(cancellationToken));
        }
        [HttpGet("{userId}")]
        [HasPermission(Permissions.GetUsers)]
        public async Task<IActionResult> GetUserDetails([FromRoute]string userId ,CancellationToken cancellationToken)
        {
            var result = await _userService.GetUserDetailsAsync(userId,cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        [HttpPost("")]
        [HasPermission(Permissions.AddUsers)]
        public async Task<IActionResult> AddUser(AddUserRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _userService.AddUserAsync(request, cancellationToken);
            return result.IsSuccess ? CreatedAtAction(nameof(GetUserDetails), new { userId =  result.Value.Id }, result.Value) : result.ToProblem();
        }
        [HttpPut("{userId}")]
        [HasPermission(Permissions.UpdateUsers)]
        public async Task<IActionResult> UpdateUser([FromRoute] string userId,UpdateUserRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _userService.UpdateUserAsync(userId,request, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
        [HttpPut("{userId}/toggle-status")]
        [HasPermission(Permissions.UpdateUsers)]
        public async Task<IActionResult> ToggleStatus([FromRoute] string userId, CancellationToken cancellationToken = default)
        {
            var result = await _userService.ToggleStatusAsync(userId, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
        [HttpPut("{userId}/unlock")]
        [HasPermission(Permissions.UpdateUsers)]
        public async Task<IActionResult> UnlockUser([FromRoute] string userId,  CancellationToken cancellationToken = default)
        {
            var result = await _userService.UnLockAsync(userId, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
    }
}

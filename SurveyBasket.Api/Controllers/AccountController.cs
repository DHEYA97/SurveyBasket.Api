using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Api.Contract.Auth.User;
using SurveyBasket.Api.Extensions;

namespace SurveyBasket.Api.Controllers
{
    [Route("/me")]
    [ApiController]
    [Authorize]
    public class AccountController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpGet("")]
        public async Task<IActionResult> Info(CancellationToken cancellationToken)
        {
            var userInfo = await _userService.GetUserAsync(User.GetUserId()!);
            return Ok(userInfo.Value);
        }
        [HttpPut("")]
        public async Task<IActionResult> Info([FromBody] UpdateProfileRequest request)
        {
            await _userService.UpdateProfileAsync(User.GetUserId()!, request);

            return NoContent();
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var result = await _userService.ChangePasswordAsync(User.GetUserId()!, request);

            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
    }
}

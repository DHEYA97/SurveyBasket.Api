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
    [Produces("application/json")]
    [Consumes("application/json")]
    public class AccountController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        /// <summary>
        /// Retrieves the authenticated user's information.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /me
        ///
        /// </remarks>
        [HttpGet("")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> Info(CancellationToken cancellationToken)
        {
            var userInfo = await _userService.GetUserAsync(User.GetUserId()!);
            return userInfo.IsSuccess ? Ok(userInfo.Value) : Problem(userInfo.Error!.Description);
        }

        /// <summary>
        /// Updates the authenticated user's profile.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /me
        ///     {
        ///        "firstName": "John",
        ///        "lastName": "Doe",
        ///     }
        ///
        /// </remarks>
        [HttpPut("")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> Info([FromBody] UpdateProfileRequest request)
        {
            var result = await _userService.UpdateProfileAsync(User.GetUserId()!, request);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }

        /// <summary>
        /// Changes the authenticated user's password.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /me/change-password
        ///     {
        ///        "CurrentPassword": "OldP@ssw0rd!",
        ///        "NewPassword": "NewP@ssw0rd!"
        ///     }
        ///
        /// </remarks>
        [HttpPut("change-password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var result = await _userService.ChangePasswordAsync(User.GetUserId()!, request);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
    }
}


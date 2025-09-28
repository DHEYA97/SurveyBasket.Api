using SurveyBasket.Api.Contract.Auth;
using SurveyBasket.Api.Contract.Auth.Register;
using SurveyBasket.Api.Contract.ConfirmEmail;
using SurveyBasket.Api.Contract.ReSendConfirmEmail;
using SurveyBasket.Api.Contract.ResetPassword;

namespace SurveyBasket.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class AuthController(IAuthService authService, ILogger<AuthController> logger) : ControllerBase
    {
        private readonly IAuthService _authService = authService;
        private readonly ILogger<AuthController> _logger = logger;

        /// <summary>
        /// Authenticates a user and generates a JWT token.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Auth/login
        ///     {
        ///        "email": "user@example.com",
        ///        "password": "P@ssw0rd!"
        ///     }
        ///
        /// </remarks>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> Login([FromBody] LoginRequest Request, CancellationToken cancellationToken = default)
        {
            var authResult = await _authService.GetTokenAsync(Request.Email, Request.Password, cancellationToken);
            return authResult.IsSuccess ? Ok(authResult.Value) : authResult.ToProblem();
        }

        /// <summary>
        /// Refreshes JWT token using a refresh token.
        /// </summary>
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest Request, CancellationToken cancellationToken = default)
        {
            var authResult = await _authService.GetRefreshTokenAsync(Request.Token, Request.RefreshToken, cancellationToken);
            return authResult.IsSuccess ? Ok(authResult.Value) : authResult.ToProblem();
        }

        /// <summary>
        /// Revokes a refresh token to prevent further use.
        /// </summary>
        [HttpPost("revoke-refresh-token")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> RevokeRefreshToken([FromBody] RefreshTokenRequest Request, CancellationToken cancellationToken = default)
        {
            var isRevoke = await _authService.RevokeRefreshTokenAsync(Request.Token, Request.RefreshToken, cancellationToken);
            return isRevoke.IsSuccess ? Ok() : isRevoke.ToProblem();
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> Register([FromBody] RegisterRequest Request, CancellationToken cancellationToken = default)
        {
            var result = await _authService.RegisterAsync(Request, cancellationToken);
            return result.IsSuccess ? Ok() : result.ToProblem();
        }

        /// <summary>
        /// Confirms user's email address.
        /// </summary>
        [HttpPost("confirm-email")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest Request, CancellationToken cancellationToken = default)
        {
            var result = await _authService.ConfirmEmailAsync(Request);
            return result.IsSuccess ? Ok() : result.ToProblem();
        }

        /// <summary>
        /// Resends confirmation email.
        /// </summary>
        [HttpPost("resend-confirm-email")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> ReSendConfirmEmail([FromBody] ReSendConfirmEmailRequest Request, CancellationToken cancellationToken = default)
        {
            var result = await _authService.ResendConfirmEmailAsync(Request);
            return result.IsSuccess ? Ok() : result.ToProblem();
        }

        /// <summary>
        /// Sends a reset password link to the user's email.
        /// </summary>
        [HttpPost("forget-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> ForgetPassword([FromBody] ReSendConfirmEmailRequest Request, CancellationToken cancellationToken = default)
        {
            var result = await _authService.ForgetPasswordAsync(Request);
            return result.IsSuccess ? Ok() : result.ToProblem();
        }

        /// <summary>
        /// Resets the user's password using a valid reset token.
        /// </summary>
        [HttpPost("reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest Request, CancellationToken cancellationToken = default)
        {
            var result = await _authService.ConfirmResetPasswordAsync(Request);
            return result.IsSuccess ? Ok() : result.ToProblem();
        }
    }
}

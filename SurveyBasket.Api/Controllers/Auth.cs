using Microsoft.Extensions.Options;
using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Authentication;
using SurveyBasket.Api.Contract.Auth;

namespace SurveyBasket.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class Auth(IAuthService authService
                        ) : ControllerBase
    {
        private readonly IAuthService _authService = authService;
        [HttpPost("")]
        public async Task<IActionResult> Login([FromBody] LoginRequest Request,CancellationToken cancellationToken = default)
        {
            var authResult = await _authService.GetTokenAsync(Request.Email, Request.Password,cancellationToken);
            return authResult.IsSuccess ? Ok(authResult.Value) : authResult.ToProblem();
        }
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest Request, CancellationToken cancellationToken = default)
        {
            var authResult = await _authService.GetRefreshTokenAsync(Request.Token, Request.RefreshToken, cancellationToken);
            return authResult.IsSuccess ? Ok(authResult.Value) : authResult.ToProblem();
        }
        [HttpPost("revoke-refresh-token")]
        public async Task<IActionResult> RevokeRefreshToken([FromBody] RefreshTokenRequest Request, CancellationToken cancellationToken = default)
        {
            var isRevoke = await _authService.RevokeRefreshTokenAsync(Request.Token, Request.RefreshToken, cancellationToken);
            return isRevoke.IsSuccess ? Ok() : isRevoke.ToProblem();
        }

    }
}

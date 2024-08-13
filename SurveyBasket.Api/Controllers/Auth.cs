using Microsoft.Extensions.Options;
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
        public async Task<IActionResult> Login(LoginRequest Request,CancellationToken cancellationToken = default)
        {       
           var auth = await _authService.GetTokenAsync(Request.Email, Request.Password,cancellationToken);
           return auth is null ? BadRequest("Email/Password Not Valid") : Ok(auth);
        }
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest Request, CancellationToken cancellationToken = default)
        {
            var auth = await _authService.GetRefreshTokenAsync(Request.Token, Request.RefreshToken, cancellationToken);
            return auth is null ? BadRequest("Token Not Valid") : Ok(auth);
        }
        [HttpPost("revoke-refresh-token")]
        public async Task<IActionResult> RevokeRefreshToken(RefreshTokenRequest Request, CancellationToken cancellationToken = default)
        {
            var isRevoke = await _authService.RevokeRefreshTokenAsync(Request.Token, Request.RefreshToken, cancellationToken);
            return isRevoke ? Ok() : BadRequest("Operation Not Complete");
        }

    }
}

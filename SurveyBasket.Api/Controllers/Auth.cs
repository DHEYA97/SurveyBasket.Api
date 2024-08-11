using SurveyBasket.Api.Contract.Auth;

namespace SurveyBasket.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class Auth(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost("")]
        public async Task<IActionResult> Login(LoginRequest Request,CancellationToken cancellationToken = default)
        {       
           var auth = await _authService.GetTokenAsync(Request.Email, Request.Password,cancellationToken);
           return auth is null ? BadRequest("Email/Password Not Valid") : Ok(auth);
        }
    }
}

using Microsoft.Extensions.Options;
using SurveyBasket.Api.Authentication;
using SurveyBasket.Api.Contract.Auth;

namespace SurveyBasket.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class Auth(IAuthService authService,
                      IOptions<JwtOptions> options,
                      IOptionsSnapshot<JwtOptions> optionsSnapshot,
                      IOptionsMonitor<JwtOptions> optionsMonitor
                        ) : ControllerBase
    {
        private readonly IAuthService _authService = authService;
        private readonly IOptions<JwtOptions> _jwtOptions = options;
        private readonly IOptionsSnapshot<JwtOptions> _optionsSnapshot = optionsSnapshot;
        private readonly IOptionsMonitor<JwtOptions> _optionsMonitor = optionsMonitor;
        [HttpPost("")]
        public async Task<IActionResult> Login(LoginRequest Request,CancellationToken cancellationToken = default)
        {       
           var auth = await _authService.GetTokenAsync(Request.Email, Request.Password,cancellationToken);
           return auth is null ? BadRequest("Email/Password Not Valid") : Ok(auth);
        }
        [HttpGet]
        public IActionResult Get() 
        {
            var j1 = new
            {
                _jwtOptions = _jwtOptions.Value.Expirition,
                _optionsSnapshot = _optionsSnapshot.Value.Expirition,
                _optionsMonitor = _optionsMonitor.CurrentValue.Expirition
            };
            Thread.Sleep(5000);
            var j2 = new
            {
                _jwtOptions = _jwtOptions.Value.Expirition,
                _optionsSnapshot = _optionsSnapshot.Value.Expirition,
                _optionsMonitor = _optionsMonitor.CurrentValue.Expirition
            };
            return Ok(new
            {
                j1,j2
            });
        }
    }
}

using SurveyBasket.Api.Authentication;
using SurveyBasket.Api.Contract.Auth;

namespace SurveyBasket.Api.Services
{
    public class AuthService(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider) : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IJwtProvider _jwtProvider = jwtProvider;

        public async Task<AuthResponse?> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return null;
            var IsValid = await _userManager.CheckPasswordAsync(user, password);
            if (!IsValid) 
                return null;
            var (token, expiration) = _jwtProvider.GenerateToken(user); 
            return new AuthResponse(user.Id,user.Email!,user.FirstName,user.LastName, token,expiration);
        }
    }
}

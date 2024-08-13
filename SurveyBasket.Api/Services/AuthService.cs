using SurveyBasket.Api.Authentication;
using SurveyBasket.Api.Contract.Auth;
using System.Security.Cryptography;

namespace SurveyBasket.Api.Services
{
    public class AuthService(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider) : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IJwtProvider _jwtProvider = jwtProvider;
        private readonly int _refreshTokenExpiredDay = 14;
        public async Task<AuthResponse?> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return null;
            var IsValid = await _userManager.CheckPasswordAsync(user, password);
            if (!IsValid) 
                return null;
            var (token, expiration) = _jwtProvider.GenerateToken(user);

            //RefreshToken
            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpierdDate = DateTime.UtcNow.AddDays(_refreshTokenExpiredDay);
            user.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                ExpiredOn = refreshTokenExpierdDate
            });
            await _userManager.UpdateAsync(user);

            return new AuthResponse(user.Id,user.Email!,user.FirstName,user.LastName, token,expiration,refreshToken,refreshTokenExpierdDate);
        }
        
        public async Task<AuthResponse?> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
        {
            if (token is null || refreshToken is null)
                return null;
            var userId = _jwtProvider.ValidateToken(token);
            if (userId is null)
                return null;
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) 
                return null;
            var userRefeshToken = user.RefreshTokens.SingleOrDefault(t=>t.Token == refreshToken && t.IsActive);
            if(userRefeshToken == null)
                return null;
            userRefeshToken.RevokedOn = DateTime.UtcNow;
            var (newToken, expiration) = _jwtProvider.GenerateToken(user);

            //RefreshToken
            var newRefreshToken = GenerateRefreshToken();
            var refreshTokenExpierdDate = DateTime.UtcNow.AddDays(_refreshTokenExpiredDay);
            user.RefreshTokens.Add(new RefreshToken
            {
                Token = newRefreshToken,
                ExpiredOn = refreshTokenExpierdDate
            });
            await _userManager.UpdateAsync(user);
            return new AuthResponse(user.Id, user.Email!, user.FirstName, user.LastName, newToken, expiration, newRefreshToken, refreshTokenExpierdDate);
        }

        public async Task<bool> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
        {
            if (token is null || refreshToken is null)
                return false;
            var userId = _jwtProvider.ValidateToken(token);
            if (userId is null)
                return false;
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;
            var userRefeshToken = user.RefreshTokens.SingleOrDefault(t => t.Token == refreshToken && t.IsActive);
            if (userRefeshToken == null)
                return false;
            userRefeshToken.RevokedOn = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
            return true;
        }

        private static string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    }
}

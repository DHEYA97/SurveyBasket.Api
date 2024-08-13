
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SurveyBasket.Api.Authentication
{
    public class JwtProvider(IOptions<JwtOptions> options) : IJwtProvider
    {
        private readonly JwtOptions _jwtOptions = options.Value;
        public (string Token, int Expirition) GenerateToken(ApplicationUser user)
        {
            Claim[] claims = [
                new (JwtRegisteredClaimNames.Sub,user.Id),
                new (JwtRegisteredClaimNames.Email,user.Email!),
                new (JwtRegisteredClaimNames.GivenName,user.FirstName),
                new (JwtRegisteredClaimNames.FamilyName,user.LastName),
                new (JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                ];
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));

            var singingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            //
            var expiresIn = _jwtOptions.Expirition;

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresIn),
                signingCredentials: singingCredentials
            );
            return (Token: new JwtSecurityTokenHandler().WriteToken(token), Expirition: expiresIn * 60);
        }

        public string? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    IssuerSigningKey = symmetricSecurityKey,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                },out SecurityToken validatedToken);
                
                var jwtToken = (JwtSecurityToken)validatedToken;
                return jwtToken.Claims.First(x=>x.Type == JwtRegisteredClaimNames.Sub).Value;
            }
            catch
            {
                return null;
            }
        }
    }
}

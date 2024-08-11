
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SurveyBasket.Api.Authentication
{
    public class JwtProvider : IJwtProvider
    {
        public (string Token, int Expirition) GenerateToken(ApplicationUser user)
        {
            Claim[] claims = [
                new (JwtRegisteredClaimNames.Sub,user.Id),
                new (JwtRegisteredClaimNames.Email,user.Email!),
                new (JwtRegisteredClaimNames.GivenName,user.FirstName),
                new (JwtRegisteredClaimNames.FamilyName,user.LastName),
                new (JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                ];
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("J7MfAb4WcAIMkkigVtIepIILOVJEjAcB"));

            var singingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            //
            var expiresIn = 30;

            var token = new JwtSecurityToken(
                issuer: "SurveyBasketApp",
                audience: "SurveyBasketApp users",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresIn),
                signingCredentials: singingCredentials
            );
            return (Token: new JwtSecurityTokenHandler().WriteToken(token), Expirition: expiresIn * 60);
        }
    }
}

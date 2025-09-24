using SurveyBasket.Api.Contract.Auth;
using Swashbuckle.AspNetCore.Filters;

namespace SurveyBasket.Api.Swagger.Example
{
    public class AuthResponseExample : IExamplesProvider<AuthResponse>
    {
        public AuthResponse GetExamples()
        {
            return new AuthResponse(
                Id: "12345",
                Email: "user@example.com",
                FirstName: "John",
                LastName: "Doe",
                Token: "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
                ExpireDate: 3600,
                RefreshToken: "b1a2c3d4e5f6g7h8i9j0",
                RefreshTokenExpireDate: DateTime.UtcNow.AddDays(7)
            );
        }
    }
}

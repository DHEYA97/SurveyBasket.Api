using SurveyBasket.Api.Contract.Auth;
using Swashbuckle.AspNetCore.Filters;

namespace SurveyBasket.Api.Swagger.Example
{
    public class LoginRequestExample : IExamplesProvider<LoginRequest>
    {
        public LoginRequest GetExamples()
        {
            return new LoginRequest
            (
                "user@example.com",
                "P@ssw0rd!"
            );
        }
    }
}

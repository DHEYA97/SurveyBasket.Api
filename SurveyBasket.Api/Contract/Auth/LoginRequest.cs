namespace SurveyBasket.Api.Contract.Auth
{
    public record LoginRequest(
        string Email,
        string Password
       );
}

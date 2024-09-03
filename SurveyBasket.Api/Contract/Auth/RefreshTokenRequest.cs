namespace SurveyBasket.Api.Contract.Auth
{
    public record RefreshTokenRequest(
        string Token,
        string RefreshToken
       );
}

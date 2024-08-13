namespace SurveyBasket.Api.Authentication
{
    public interface IJwtProvider
    {
        (string Token,int Expirition) GenerateToken(ApplicationUser applicationUser);
        string? ValidateToken(string token);
    }
}

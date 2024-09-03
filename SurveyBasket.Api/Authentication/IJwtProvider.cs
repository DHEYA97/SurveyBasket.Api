namespace SurveyBasket.Api.Authentication
{
    public interface IJwtProvider
    {
        (string Token,int Expirition) GenerateToken(ApplicationUser applicationUser, IEnumerable<string> roles, IEnumerable<string> permissions);
        string? ValidateToken(string token);
    }
}

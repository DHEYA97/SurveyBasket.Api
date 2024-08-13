namespace SurveyBasket.Api.Contract.Auth
{
    public record AuthResponse(
         string Id,
         string Email,
         string FirstName,
         string LastName,
         string Token,
         int ExpireDate,
         string RefreshToken,
         DateTime RefreshTokenExpireDate
        );
}

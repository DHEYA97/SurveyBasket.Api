namespace SurveyBasket.Api.Contract.Auth.Register
{
    public record RegisterRequest(
        string Email,
        string Password,
        string FirstName,
        string LastName
        );
}

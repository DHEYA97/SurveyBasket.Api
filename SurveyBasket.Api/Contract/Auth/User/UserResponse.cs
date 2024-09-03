namespace SurveyBasket.Api.Contract.Auth.User
{
    public record UserResponse(
         string Email,
         string UserName,
         string FirstName,
         string LastName
        );
}

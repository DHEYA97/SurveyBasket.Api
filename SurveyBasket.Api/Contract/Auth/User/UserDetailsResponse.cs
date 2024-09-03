namespace SurveyBasket.Api.Contract.Auth.User
{
    public record UserDetailsResponse(
        string Id, 
        string FirstName,
        string LastName,
        string Email,
        bool IsDisabled,
        IList<string>Roles
        );
}

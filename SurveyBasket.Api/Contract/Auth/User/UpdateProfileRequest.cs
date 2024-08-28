namespace SurveyBasket.Api.Contract.Auth.User;

public record UpdateProfileRequest(
    string FirstName,
    string LastName
);
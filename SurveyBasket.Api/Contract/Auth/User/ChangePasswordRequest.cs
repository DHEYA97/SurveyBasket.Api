namespace SurveyBasket.Api.Contract.Auth.User;

public record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword
);
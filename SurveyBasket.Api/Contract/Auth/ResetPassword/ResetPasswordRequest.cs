namespace SurveyBasket.Api.Contract.ResetPassword
{
    public record ResetPasswordRequest(
        string Email,
        string Code,
        string NewPassword
        );
}

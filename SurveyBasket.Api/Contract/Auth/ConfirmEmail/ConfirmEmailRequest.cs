namespace SurveyBasket.Api.Contract.ConfirmEmail
{
    public record ConfirmEmailRequest(
        string UserId,
        string Code
        );
}

namespace SurveyBasket.Api.Contract.Roles
{
    public record RolesResponse(
        string Id,
        string Name,
        bool IsDeleted
    );
}

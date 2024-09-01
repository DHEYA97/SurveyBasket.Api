namespace SurveyBasket.Api.Contract.Roles
{
    public record RoleWithPermissionsRequest(
        string Name,
        IList<string> Permissions
    );
}

namespace SurveyBasket.Api.Contract.Roles
{
    public record RoleWithPermissionsResponse(
        string Id,
        string Name,
        bool IsDeleted,
        IList<string> Permissions
        );
    
}

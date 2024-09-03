namespace SurveyBasket.Api.Errors
{
    public static class RoleErrors
    {
        public static readonly Error RoleNotFound =
            new("Role.NotFound", "No Role was found with the given ID",StatusCodes.Status404NotFound);
        public static readonly Error DuplicateRole =
           new("Role.DuplicateTitle", "Another Role with The Same Name", StatusCodes.Status409Conflict);
        public static readonly Error InvalidPermissions =
       new("Role.InvalidPermissions", "Invalid permissions", StatusCodes.Status400BadRequest);
    }
}

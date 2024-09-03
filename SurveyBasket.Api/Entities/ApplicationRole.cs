namespace SurveyBasket.Api.Entities
{
    public sealed class ApplicationRole : IdentityRole
    {
        public bool IsDefault { get; set; }
        public bool IsDeleted { get; set; }
    }
}

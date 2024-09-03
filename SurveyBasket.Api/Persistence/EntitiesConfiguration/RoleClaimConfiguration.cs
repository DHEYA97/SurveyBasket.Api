using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SurveyBasket.Api.Abstractions.Consts;

namespace SurveyBasket.Api.Persistence.EntitiesConfiguration
{
    public class RoleClaimConfiguration : IEntityTypeConfiguration<IdentityRoleClaim<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityRoleClaim<string>> builder)
        {

            //Seeding data
            var permissionsList = Permissions.GetAllPermissions();
            IList<IdentityRoleClaim<string>> roleClaim = [];
            int i = 0;
            foreach (var permission in permissionsList)
            {
                roleClaim.Add(
                    new IdentityRoleClaim<string>
                    {
                        Id = ++i,
                        ClaimType = Permissions.Type,
                        ClaimValue = permission,
                        RoleId = DefaultRoles.AdminRoleId,
                    });
            }
            builder.HasData(roleClaim);
        }
    } 
}

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SurveyBasket.Api.Abstractions.Consts;

namespace SurveyBasket.Api.Persistence.EntitiesConfiguration
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
        {

            //Seeding data

            builder.HasData(
                     new IdentityUserRole<string>{
                           UserId = DefaultUsers.AdminId,
                           RoleId = DefaultRoles.AdminRoleId
                       }
                );
        }
    }
}

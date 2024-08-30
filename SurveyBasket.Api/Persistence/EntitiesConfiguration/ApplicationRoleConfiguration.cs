using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SurveyBasket.Api.Abstractions.Consts;

namespace SurveyBasket.Api.Persistence.EntitiesConfiguration
{
    public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {
            
            //Seeding data
            
            builder.HasData(
                   [
                       new ApplicationRole{
                           Id = DefaultRoles.AdminRoleId,
                           ConcurrencyStamp = DefaultRoles.AdminRoleConcurrencyStamp,
                           Name = DefaultRoles.Admin,
                           NormalizedName = DefaultRoles.Admin.ToUpper(),
                       },
                       new ApplicationRole{
                           Id = DefaultRoles.MemberRoleId,
                           ConcurrencyStamp = DefaultRoles.MemberRoleConcurrencyStamp,
                           Name = DefaultRoles.Member,
                           NormalizedName = DefaultRoles.Member.ToUpper(),
                           IsDefault = true,
                       },
                   ]
                );
        }
    }
}

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SurveyBasket.Api.Persistence.EntitiesConfiguration
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasMany(u => u.RefreshTokens)
                   .WithOne(rt => rt.User)
                   .HasForeignKey(rt => rt.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(u => u.FirstName).HasMaxLength(100);
            builder.Property(u => u.LastName).HasMaxLength(100);

            //Seeding data
            builder.HasData(
                new ApplicationUser
                {
                    Id = DefaultUsers.AdminId,
                    FirstName = "Survey Basket",
                    LastName = "Admin",
                    UserName = DefaultUsers.AdminEmail,
                    NormalizedUserName = DefaultUsers.AdminEmail.ToUpper(),
                    Email = DefaultUsers.AdminEmail,
                    NormalizedEmail = DefaultUsers.AdminEmail.ToUpper(),
                    SecurityStamp = DefaultUsers.AdminSecurityStamp,
                    ConcurrencyStamp = DefaultUsers.AdminConcurrencyStamp,
                    PasswordHash = DefaultUsers.AdminPasswordHash,
                    EmailConfirmed = true
                }
                );
        }
    }
}

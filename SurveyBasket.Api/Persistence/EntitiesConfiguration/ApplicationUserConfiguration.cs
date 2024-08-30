using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SurveyBasket.Api.Abstractions.Consts;

namespace SurveyBasket.Api.Persistence.EntitiesConfiguration
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.OwnsMany(u => u.RefreshTokens)
                   .ToTable("RefreshTokens")
                   .WithOwner()
                   .HasForeignKey("UserId");

            builder.Property(u => u.FirstName).HasMaxLength(100);
            builder.Property(u=>u.LastName).HasMaxLength(100);

            //Seeding data
            var passwordHasher = new PasswordHasher<ApplicationUser>();
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
                    PasswordHash = passwordHasher.HashPassword(null!,DefaultUsers.AdminPassword),
                    EmailConfirmed = true
                }
                );
        }
    }
}

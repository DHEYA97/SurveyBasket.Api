using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
        }
    }
}

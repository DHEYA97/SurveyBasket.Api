using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SurveyBasket.Api.Persistence.EntitiesConfiguration
{
    public class QuestionConfiguration : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            builder.HasIndex(p => new { p.PollId, p.Content }).IsUnique();
            builder.Property(q => q.Content).HasMaxLength(1000);
        }
    }
}

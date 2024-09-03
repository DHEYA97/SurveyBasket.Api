using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SurveyBasket.Api.Persistence.EntitiesConfiguration
{
    public class AnswerConfiguration : IEntityTypeConfiguration<Answer>
    {
        public void Configure(EntityTypeBuilder<Answer> builder)
        {
            builder.HasIndex(a => new { a.QuestionId, a.Content }).IsUnique();
            builder.Property(a => a.Content).HasMaxLength(1000);
        }
    }
}

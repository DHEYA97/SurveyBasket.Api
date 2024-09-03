using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SurveyBasket.Api.Extensions;
using System.Reflection;
using System.Security.Claims;

namespace SurveyBasket.Api.Persistence
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,IHttpContextAccessor httpContextAccessor): 
        IdentityDbContext<ApplicationUser,ApplicationRole,string>(options)
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Poll> Polls { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<VoteAnswer> VoteAnswers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Add Global Configurations
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Change OnDelete Behavior
            var cascadeFk = modelBuilder.Model
                                        .GetEntityTypes()
                                        .SelectMany(t => t.GetForeignKeys())
                                        .Where(fk => fk.DeleteBehavior == DeleteBehavior.Cascade);
            foreach (var fk in cascadeFk)
                fk.DeleteBehavior = DeleteBehavior.Restrict;

            base.OnModelCreating(modelBuilder);
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var Entities = ChangeTracker.Entries<AuditEntity>();
            foreach (var entity in Entities)
            {
                var currentUserId = _httpContextAccessor.HttpContext?.User.GetUserId()!;
                if(entity.State == EntityState.Added)
                {
                    entity.Property(x=>x.CreatedById).CurrentValue = currentUserId;
                }
                else if (entity.State == EntityState.Modified)
                {
                    entity.Property(x => x.UpdatedById).CurrentValue = currentUserId;
                    entity.Property(x => x.UpdatedOn).CurrentValue = DateTime.UtcNow;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}

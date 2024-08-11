using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace SurveyBasket.Api.Persistence
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): IdentityDbContext<ApplicationUser>(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Add Global
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Poll> Polls { get; set; }
    }
}

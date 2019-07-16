namespace Soccer.Core.Domain
{
    using Microsoft.EntityFrameworkCore;
    using Soccer.Core.Domain.Matches.Entities;

    public class SoccerContext : DbContext
    {
        public SoccerContext(DbContextOptions<SoccerContext> options)
            : base(options)
        {
        }

        public DbSet<MatchEntity> Match { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new MatchEntityConfiguration());
        }
    }
}
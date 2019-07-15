using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Soccer.Core.Infrastructure.Entities;

namespace Soccer.Core.Infrastructure
{
    internal class SoccerContext : DbContext
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
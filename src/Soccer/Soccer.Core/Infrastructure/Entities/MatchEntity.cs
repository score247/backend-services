﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Soccer.Core.Infrastructure.Entities
{
    public class MatchEntity : BaseEntity
    {
        [Column(TypeName = "jsonb")]
        public string Value { get; set; }

        public DateTimeOffset EventDate { get; set; }

        public string LeagueId { get; set; }

        [Key]
        public string Language { get; set; }

        public int SportId { get; set; }

        public string Region { get; set; }
    }

    public class MatchEntityConfiguration : IEntityTypeConfiguration<MatchEntity>
    {
        public void Configure(EntityTypeBuilder<MatchEntity> builder)
        {
            builder.HasKey(m => new { m.Id, m.Language });
        }
    }
}
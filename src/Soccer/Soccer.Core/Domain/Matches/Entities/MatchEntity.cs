namespace Soccer.Core.Domain.Matches.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Newtonsoft.Json;
    using Score247.Shared.Base;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Domain.Matches.Models;

    public class MatchEntity : BaseEntity
    {
        [Column(TypeName = "jsonb")]
        public string Value { get; set; }

        public DateTimeOffset EventDate { get; set; }

        public string LeagueId { get; set; }

        public string Language { get; set; }

        public int SportId { get; set; }

        public string Region { get; set; }

        [NotMapped]
        public Match Match
        {
            get => JsonConvert.DeserializeObject<Match>(Value);
            set
            {
                Id = value.Id;
                LeagueId = value.League.Id;
                Region = value.Region;
                SportId = int.Parse(Sport.Soccer.Value);
                EventDate = value.EventDate.ToUniversalTime();
                Value = JsonConvert.SerializeObject(value);
            }
        }
    }

    public class MatchEntityConfiguration : IEntityTypeConfiguration<MatchEntity>
    {
        public void Configure(EntityTypeBuilder<MatchEntity> builder)
        {
            builder.HasKey(m => new { m.Id, m.Language });
        }
    }
}
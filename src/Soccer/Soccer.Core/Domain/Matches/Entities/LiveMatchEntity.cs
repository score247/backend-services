﻿namespace Soccer.Core.Domain.Matches.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using Newtonsoft.Json;
    using Score247.Shared.Base;
    using Soccer.Core.Domain.Matches.Models;

    public class LiveMatchEntity : BaseEntity
    {
        [Column(TypeName = "jsonb")]
        public string Value { get; set; }

        public DateTimeOffset EventDate { get; set; }

        public string LeagueId { get; set; }

        public string Language { get; set; }

        public int SportId { get; set; }

        [NotMapped]
        public Match Match
        {
            get => JsonConvert.DeserializeObject<Match>(Value);
            set => Value = JsonConvert.SerializeObject(value);
        }
    }
}
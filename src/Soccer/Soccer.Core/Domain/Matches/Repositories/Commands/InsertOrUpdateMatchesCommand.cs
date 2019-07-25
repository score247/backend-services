﻿namespace Soccer.Core.Domain.Matches.Repositories.Commands
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Newtonsoft.Json;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Domain.Matches.Models;

    public class InsertOrUpdateMatchesCommand : NonQueryCommand
    {
        public InsertOrUpdateMatchesCommand(IEnumerable<Match> matches, string language)
        {
            SportId = int.Parse(Sport.Soccer.Value);
            Matches = JsonConvert.SerializeObject(
                matches,
                new JsonSerializerSettings()
                {
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                });

            Language = language;
        }

        public int SportId { get; }

        public string Matches { get; }

        public string Language { get; }

        public override string GetSettingKey() => "Score247_InsertOrUpdateMatches";

        public override bool IsValid() => !string.IsNullOrEmpty(Matches) && !string.IsNullOrEmpty(Language);
    }
}
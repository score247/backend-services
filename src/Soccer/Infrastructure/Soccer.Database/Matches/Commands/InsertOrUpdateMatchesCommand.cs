using System;
using System.Collections.Generic;
using Score247.Shared.Enumerations;
using Soccer.Core.Matches.Models;
using Soccer.Database._Shared.Extensions;

namespace Soccer.Database.Matches.Commands
{
    public class InsertOrUpdateMatchesCommand : BaseCommand
    {
        private const string SpName = "Match_InsertMatches";

        public InsertOrUpdateMatchesCommand(
            IEnumerable<Match> matches,
            string language,
            DateTimeOffset eventDate = default) : base(eventDate)
        {
            SportId = Sport.Soccer.Value;
            Matches = ToJsonString(matches);
            Language = language;
        }

        public byte SportId { get; }

        public string Matches { get; }

        public string Language { get; }

        public override string GetSettingKey() => SpName.GetCorrespondingKey(EventDate, DateTimeOffset.Now);

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Matches) && !string.IsNullOrWhiteSpace(Language);
    }
}
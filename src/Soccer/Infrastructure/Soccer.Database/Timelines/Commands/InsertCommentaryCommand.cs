using System;
using System.Collections.Generic;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.Database._Shared.Extensions;

namespace Soccer.Database.Timelines.Commands
{
    public class InsertCommentaryCommand : BaseCommand
    {
        public InsertCommentaryCommand(
          string matchId,
          long timelineId,
          IReadOnlyList<Commentary> commentaries,
          Language language,
          DateTimeOffset eventDate = default)
            : base(eventDate)
        {
            MatchId = matchId;
            TimelineId = timelineId;
            Commentaries = ToJsonString(commentaries);
            Language = language.DisplayName;
        }

        public string MatchId { get; }

        public long TimelineId { get; }

        public string Commentaries { get; }

        public string Language { get; }

        public override string GetSettingKey() => "Match_InsertCommentary".GetCorrespondingKey(EventDate, DateTimeOffset.Now);

        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(MatchId)
                && TimelineId > 0
                && !string.IsNullOrWhiteSpace(Commentaries);
    }
}

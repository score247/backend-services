using System.Collections.Generic;
using System.Linq;
using Score247.Shared.Enumerations;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Database.Matches.Commands
{
    public class InsertOrRemoveLiveMatchesCommand : BaseCommand
    {
        public InsertOrRemoveLiveMatchesCommand(
            Language language,
            IEnumerable<Match> newMatches,
            IEnumerable<Match> removedMatches)

        {
            SportId = Sport.Soccer.Value;
            Language = language.DisplayName;
            NewMatches = ToJsonString(newMatches.Select(x => new { MatchId = x.Id, x.MatchResult }));
            RemovedMatchIds = ToJsonString(removedMatches.Select(x => new { MatchId = x.Id }));
        }

        public byte SportId { get; }

        public string Language { get; }

        public string NewMatches { get; }

        public string RemovedMatchIds { get; }

        public override string GetSettingKey() => "LiveMatch_InsertOrRemove";

        public override bool IsValid()
            => SportId > 0
                && !string.IsNullOrWhiteSpace(Language)
                && (!string.IsNullOrWhiteSpace(NewMatches) || !string.IsNullOrWhiteSpace(RemovedMatchIds));
    }
}

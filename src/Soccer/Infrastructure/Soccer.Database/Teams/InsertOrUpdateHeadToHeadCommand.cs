using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Database.Teams
{
    public class InsertOrUpdateHeadToHeadCommand : BaseCommand
    {
        public InsertOrUpdateHeadToHeadCommand(string homeTeamId, string awayTeamId, Match match, Language language)
        {
            HomeTeamId = homeTeamId;
            AwayTeamId = awayTeamId;
            Match = ToJsonString(match);
            MatchId = match.Id;
            Language = language.DisplayName;
        }

        public string HomeTeamId { get; }

        public string AwayTeamId { get; }

        public string Match { get; }

        public string MatchId { get; }

        public string Language { get; }

        public override string GetSettingKey()
            => "Team_InsertOrUpdateHeadToHead";

        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(HomeTeamId)
               && !string.IsNullOrWhiteSpace(AwayTeamId)
               && !string.IsNullOrWhiteSpace(MatchId)
               && !string.IsNullOrWhiteSpace(Match);
    }
}
using Soccer.Core.Matches.Models;

namespace Soccer.Database.Teams
{
    public class InsertOrUpdateHeadToHeadCommand : BaseCommand
    {
        public InsertOrUpdateHeadToHeadCommand(string homeTeamId, string awayTeamId, Match match)
        {
            HomeTeamId = homeTeamId;
            AwayTeamId = awayTeamId;
            Match = ToJsonString(match);
            MatchId = match.Id;
        }

        public string HomeTeamId { get; }

        public string AwayTeamId { get; }

        public string Match { get; }

        public string MatchId { get; }

        public override string GetSettingKey()
            => "Team_InsertOrUpdateHeadToHead";

        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(HomeTeamId)
               && !string.IsNullOrWhiteSpace(AwayTeamId)
               && !string.IsNullOrWhiteSpace(MatchId)
               && !string.IsNullOrWhiteSpace(Match);
    }
}
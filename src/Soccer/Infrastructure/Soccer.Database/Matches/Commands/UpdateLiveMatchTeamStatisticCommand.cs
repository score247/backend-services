namespace Soccer.Database.Matches.Commands
{
    using Score247.Shared.Enumerations;
    using Soccer.Core.Teams.Models;

    public class UpdateLiveMatchTeamStatisticCommand : BaseCommand
    {
        public UpdateLiveMatchTeamStatisticCommand(string matchId, bool isHome, TeamStatistic statistic)
        {
            SportId = Sport.Soccer.Value;
            MatchId = matchId;
            TeamIndex = isHome ? 0 : 1;
            Statistic = ToJsonString(statistic);
        }

        public byte SportId { get; }

        public string MatchId { get; }

        public int TeamIndex { get; }

        public string Statistic { get; }

        public override string GetSettingKey() => "LiveMatch_UpdateTeamStatistic";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(MatchId) && !string.IsNullOrEmpty(Statistic);
    }
}
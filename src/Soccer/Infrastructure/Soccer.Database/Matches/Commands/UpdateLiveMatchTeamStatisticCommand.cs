namespace Soccer.Database.Matches.Commands
{
    using System;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Teams.Models;
    using Soccer.Database._Shared.Extensions;

    public class UpdateLiveMatchTeamStatisticCommand : BaseCommand
    {
        public UpdateLiveMatchTeamStatisticCommand(
            string matchId, 
            bool isHome, 
            TeamStatistic statistic,
            DateTimeOffset eventDate = default)
            : base(eventDate)
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

        public override string GetSettingKey() => "LiveMatch_UpdateTeamStatistic".GetCorrespondingKey(EventDate);

        public override bool IsValid() => !string.IsNullOrWhiteSpace(MatchId) && !string.IsNullOrEmpty(Statistic);
    }
}
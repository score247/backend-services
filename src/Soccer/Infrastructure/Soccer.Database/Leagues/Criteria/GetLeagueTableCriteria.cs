using Fanex.Data.Repository;
using Soccer.Core._Shared.Enumerations;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Database.Leagues.Criteria
{
    public class GetLeagueTableCriteria : CriteriaBase
    {
        public GetLeagueTableCriteria(
            string leagueId,
            string seasonId,
            Language language)
        {
            LeagueId = leagueId;
            SeasonId = seasonId;
            Language = language?.DisplayName;
        }

        public string LeagueId { get; }

        public string SeasonId { get; }

        public string Language { get; }

        public string TableType { get; } = LeagueTableType.Total;

        public override string GetSettingKey()
            => "League_GetStandings";

        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(LeagueId)
                && !string.IsNullOrWhiteSpace(SeasonId)
                && !string.IsNullOrWhiteSpace(Language);
    }
}
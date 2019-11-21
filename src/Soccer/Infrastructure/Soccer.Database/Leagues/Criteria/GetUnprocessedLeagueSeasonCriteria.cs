using Fanex.Data.Repository;

namespace Soccer.Database.Leagues.Criteria
{
    public class GetUnprocessedLeagueSeasonCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "League_GetUnprocessedLeagueSeason";

        public override bool IsValid() => true;
    }
}

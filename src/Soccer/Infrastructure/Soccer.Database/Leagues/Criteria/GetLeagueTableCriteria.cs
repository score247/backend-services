using System;
using System.Collections.Generic;
using System.Text;
using Fanex.Data.Repository;

namespace Soccer.Database.Leagues.Criteria
{
    public class GetLeagueTableCriteria : CriteriaBase
    {
        public string LeagueId { get; }
        public string SeasonId { get; }
        public string TableType { get; }
        public string Language { get; }

        public override string GetSettingKey()
            => "League_GetStandings";

        public override bool IsValid()
            => !string.IsNullOrEmpty(LeagueId)
                && !string.IsNullOrEmpty(SeasonId)
                && !string.IsNullOrEmpty(TableType);
    }
}
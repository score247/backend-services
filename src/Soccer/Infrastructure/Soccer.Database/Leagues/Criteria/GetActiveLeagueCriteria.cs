using Fanex.Data.Repository;

namespace Soccer.Database.Leagues.Criteria
{
    public class GetActiveLeagueCriteria : CriteriaBase
    {
        public override string GetSettingKey()
            => "League_GetActive";

        public override bool IsValid()
            => true;
    }
}
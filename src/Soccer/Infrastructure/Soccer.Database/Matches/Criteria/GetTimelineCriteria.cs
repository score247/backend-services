namespace Soccer.Database.Matches.Criteria
{
    using Fanex.Data.Repository;

    public class GetTimelineCriteria : CriteriaBase
    {
        public GetTimelineCriteria(string matchId)
        {
            MatchId = matchId;
        }

        public string MatchId { get; }

        public override string GetSettingKey() => "Score247_GetTimeline";

        public override bool IsValid() => !string.IsNullOrEmpty(MatchId);
    }
}
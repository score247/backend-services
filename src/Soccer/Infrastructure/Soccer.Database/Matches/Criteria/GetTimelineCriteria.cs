namespace Soccer.Database.Matches.Criteria
{
    using Fanex.Data.Repository;

    public class GetTimelineEventsCriteria : CriteriaBase
    {
        public GetTimelineEventsCriteria(string matchId)
        {
            MatchId = matchId;
        }

        public string MatchId { get; }

        public override string GetSettingKey() => "Match_GetTimelineEvents";

        public override bool IsValid() => !string.IsNullOrEmpty(MatchId);
    }
}
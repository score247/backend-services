namespace Soccer.Database.Odds.Criteria
{
    using Fanex.Data.Repository;

    public class GetOddsCriteria : CriteriaBase
    {
        public GetOddsCriteria(string matchId, int betTypeId, string bookmakerId)
        {
            MatchId = matchId;
            BetTypeId = betTypeId;
            BookmakerId = bookmakerId;
        }

        public string MatchId { get; }

        public int BetTypeId { get; }

        public string BookmakerId { get; }

        public override string GetSettingKey()
            => "Score247_Odds_GetOdds";

        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(MatchId)
                && BetTypeId > 0
                && !string.IsNullOrWhiteSpace(BookmakerId);
    }
}
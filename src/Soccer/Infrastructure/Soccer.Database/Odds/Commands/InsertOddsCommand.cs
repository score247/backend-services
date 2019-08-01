namespace Soccer.Database.Odds.Commands
{
    using System;
    using System.Collections.Generic;
    using Soccer.Core.Odds.Models;

    public class InsertOddsCommand : BaseCommand
    {
        public InsertOddsCommand(
            IEnumerable<BetTypeOdds> betTypeOdds,
            string matchId,
            string bookmakerId,
            int betTypeId)
        {
            OddsList = ToJsonString(betTypeOdds);
            MatchId = matchId;
            BookmakerId = bookmakerId;
            BetTypeId = betTypeId;
        }

        public string OddsList { get; }

        public int BetTypeId { get; }

        public string BookmakerId { get; }

        public string MatchId { get; }

        public DateTimeOffset CreatedTime { get; } = DateTimeOffset.Now;

        public override string GetSettingKey()
            => "Score247_Odds_InsertOdds";

        public override bool IsValid()
            => BetTypeId > 0
                && !string.IsNullOrWhiteSpace(BookmakerId)
                && !string.IsNullOrWhiteSpace(MatchId)
                && !string.IsNullOrWhiteSpace(OddsList);
    }
}
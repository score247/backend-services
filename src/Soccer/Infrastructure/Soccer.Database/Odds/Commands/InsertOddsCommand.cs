namespace Soccer.Database.Odds.Commands
{
    using System.Collections.Generic;
    using Soccer.Core.Odds.Models;

    public class InsertOddsCommand : BaseCommand
    {
        public InsertOddsCommand(
            IEnumerable<BetTypeOdds> betTypeOdds,
            string matchId)
        {
            OddsList = ToJsonString(betTypeOdds);
            MatchId = matchId;
        }

        public string OddsList { get; }

        public string MatchId { get; }

        public override string GetSettingKey()
            => "Odds_InsertOdds";

        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(MatchId)
                && !string.IsNullOrWhiteSpace(OddsList);
    }
}
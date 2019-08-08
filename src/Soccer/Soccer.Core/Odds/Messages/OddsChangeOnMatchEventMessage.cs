namespace Soccer.Core.Odds.Messages
{
    using Soccer.Core.Odds.Models;

    public interface IOddsChangeOnMatchEventMessage
    {
        MatchOdds MatchOdds { get; }
    }

    public class OddsChangeOnMatchEventMessage : IOddsChangeOnMatchEventMessage
    {
        public OddsChangeOnMatchEventMessage(MatchOdds matchOdds)
        {
            MatchOdds = matchOdds;
        }

        public MatchOdds MatchOdds { get; }
    }
}
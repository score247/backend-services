namespace Soccer.Core.Odds.Messages
{
    using System.Collections.Generic;
    using Soccer.Core.Odds.Models;

    public interface IMatchOddsMessage
    {
        IEnumerable<MatchOdds> MatchOddsList { get; }
    }

    public class MatchOddsMessage : IMatchOddsMessage
    {
        public MatchOddsMessage(IEnumerable<MatchOdds> matchOddsList)
        {
            MatchOddsList = matchOddsList;
        }

        public IEnumerable<MatchOdds> MatchOddsList { get; }
    }
}
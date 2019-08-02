namespace Soccer.Core.Odds.Messages
{
    using System.Collections.Generic;
    using Soccer.Core.Odds.Models;

    public interface IOddsChangeMessage
    {
        IEnumerable<MatchOdds> MatchOddsList { get; }

        bool IsForceInsert { get; }
    }

    public class OddsChangeMessage : IOddsChangeMessage
    {
        public OddsChangeMessage(
            IEnumerable<MatchOdds> matchOddsList,
            bool isForceInsert = false)
        {
            MatchOddsList = matchOddsList;
            IsForceInsert = isForceInsert;
        }

        public IEnumerable<MatchOdds> MatchOddsList { get; }

        public bool IsForceInsert { get; }
    }
}
using MessagePack;
using Soccer.Core.Matches.Models;

namespace Soccer.API.Matches.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class MatchCoverage
    {
        public MatchCoverage(string matchId, Coverage coverage)
        {
            MatchId = matchId;
            Coverage = coverage;
        }

        public string MatchId { get; }

        public Coverage Coverage { get; }
    }
}
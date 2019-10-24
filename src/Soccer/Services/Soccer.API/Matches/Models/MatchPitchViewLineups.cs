using MessagePack;
using Soccer.Core.Matches.Models;

namespace Soccer.API.Matches.Models
{
    [MessagePackObject]
    public class MatchPitchViewLineups
    {
        public MatchPitchViewLineups(MatchLineups matchLineups, string pitchView)
        {
            MatchLineups = matchLineups;
            PitchView = pitchView;
        }

        [Key(0)]
        public MatchLineups MatchLineups { get; }

        [Key(1)]
        public string PitchView { get; }
    }
}
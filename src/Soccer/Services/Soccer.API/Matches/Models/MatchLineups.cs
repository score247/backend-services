using MessagePack;
using Soccer.Core.Teams.Models;

namespace Soccer.API.Matches.Models
{
    [MessagePackObject]
    public class MatchLineups
    {
        public MatchLineups(Team home, Team away, string pitchView)
        {
            Home = home;
            Away = away;
            PitchView = pitchView;
        }

        [Key(0)]
        public Team Home { get; }

        [Key(1)]
        public Team Away { get; }

        [Key(2)]
        public string PitchView { get; }
    }
}

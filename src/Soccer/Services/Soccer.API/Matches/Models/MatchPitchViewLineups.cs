using System;
using MessagePack;
using Soccer.Core.Teams.Models;

namespace Soccer.API.Matches.Models
{
    [MessagePackObject]
    public class MatchPitchViewLineups
    {
        public MatchPitchViewLineups(
            string id,
            DateTimeOffset eventDate,
            TeamLineups home,
            TeamLineups away,
            string pitchView)
        {
            Id = id;
            EventDate = eventDate;
            Home = home;
            Away = away;
            PitchView = pitchView;
        }
#pragma warning disable S109 // Magic numbers should not be used
        [Key(0)]
        public string Id { get; }

        [Key(1)]
        public DateTimeOffset EventDate { get; }

        [Key(2)]
        public TeamLineups Home { get; }

        [Key(3)]
        public TeamLineups Away { get; }

        [Key(4)]
        public string PitchView { get; }
#pragma warning restore S109 // Magic numbers should not be used
    }
}
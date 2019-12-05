using System;
using MessagePack;
using Soccer.Core.Teams.Models;

namespace Soccer.API.Matches.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
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
        public string Id { get; }

        public DateTimeOffset EventDate { get; }

        public TeamLineups Home { get; }

        public TeamLineups Away { get; }

        public string PitchView { get; }
#pragma warning restore S109 // Magic numbers should not be used
    }
}
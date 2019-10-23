using MessagePack;

namespace Soccer.Core.Matches.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class Coverage
    {
        public Coverage(
            bool live,
            string trackerWidgetLink,
            bool basicScore = false,
            bool keyEvents = false,
            bool detailedEvents = false,
            bool lineups = false,
            bool commentary = false)
        {
            Live = live;
            BasicScore = basicScore;
            KeyEvents = keyEvents;
            DetailedEvents = detailedEvents;
            Lineups = lineups;
            Commentary = commentary;
            TrackerWidgetLink = trackerWidgetLink;
        }

        public bool Live { get; }

        public bool BasicScore { get; }

        public bool KeyEvents { get; }

        public bool DetailedEvents { get; }

        public bool Lineups { get; }

        public bool Commentary { get; }

        public string TrackerWidgetLink { get; }
    }
}
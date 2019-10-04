using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Core.Matches.QueueMessages
{
    public interface ITimelineFetchedMessage
    {
        string MatchId { get; }

        TimelineEvent Timeline { get; }

        Language Language { get; }
    }

    public class TimelineFetchedMessage : ITimelineFetchedMessage
    {
        public TimelineFetchedMessage(string matchId, TimelineEvent timeline, Language language) 
        {
            MatchId = matchId;           
            Language = language;
            Timeline = timeline;
        }

        public string MatchId { get; }

        public TimelineEvent Timeline { get; }

        public Language Language { get; }
    }
}

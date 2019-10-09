using System;
using System.Collections.Generic;
using System.Text;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.Core.Timeline.QueueMessages
{
    public interface ITimelineUpdatedMessage
    {
        string MatchId { get; }

        string LeagueId { get; }

        Language Language { get; }

        TimelineEvent Timeline { get; }
    }
   
    public class TimelineUpdatedMessage : ITimelineUpdatedMessage
    {
        public TimelineUpdatedMessage(string matchId, string leagueId, Language language, TimelineEvent timeline)
        {
            MatchId = matchId;
            LeagueId = leagueId;
            Language = language;
            Timeline = timeline;
        }

        public string MatchId { get; }

        public string LeagueId { get; }

        public Language Language { get; }

        public TimelineEvent Timeline { get; }
    }
}

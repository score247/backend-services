using System;
using Soccer.Core.Matches.Models;
using Soccer.Core.Teams.Models;

namespace Soccer.EventProcessors.Notifications.Models
{
    public class ScoreChangeNotification : TimelineNotification
    {
        public ScoreChangeNotification(
         TimelineEvent timeline,
         Team home,
         Team away,
         string matchTime,
         MatchResult matchResult) : base(timeline, home, away, matchTime, matchResult) { }

        protected override string Content()
        {
            throw new NotImplementedException();
        }

        protected override string Title()
        {
            throw new NotImplementedException();
        }
    }
}

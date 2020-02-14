using System;
using System.Collections.Generic;
using Soccer.Core._Shared.Resources;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.Core.Teams.Models;

namespace Soccer.EventProcessors.Notifications.Models
{
    public static class TimelineNotificationCreator
    {
        private static Dictionary<EventType, Type> factories = new Dictionary<EventType, Type>
        {
            { EventType.MatchStarted, typeof(MatchStartedNotification)  },
            { EventType.MatchEnded, typeof(MatchEndNotification)  },
            { EventType.RedCard, typeof(RedCardNotification)  },
            { EventType.YellowRedCard, typeof(YellowRedCardNotification)  },
            { EventType.ScoreChange, typeof(ScoreChangeNotification)  },
            { EventType.ScoreChangeByOwnGoal, typeof(ScoreChangeNotification)  },
            { EventType.ScoreChangeByPenalty, typeof(ScoreChangeNotification)  }
        };

        public static TimelineNotification CreateInstance(
            ILanguageResourcesService languageResources,
            EventType timelineType,
            TimelineEvent timeline,
            Team home,
            Team away,
            byte matchTime = 0,
            MatchResult matchResult = null)
            => factories.ContainsKey(timelineType)
                    ? Activator.CreateInstance(
                        factories[timelineType],
                        languageResources,
                        timeline,
                        home,
                        away,
                        matchTime,
                        matchResult) as TimelineNotification
                    : null;
    }
}

﻿using Soccer.Core.Matches.Models;
using Soccer.Core.Teams.Models;
using Soccer.EventProcessors.Notifications.Constants;

namespace Soccer.EventProcessors.Notifications.Models
{
    public class MatchStartedNotification : TimelineNotification
    {
        public MatchStartedNotification(
            TimelineEvent timeline,
            Team home,
            Team away,
            byte matchTime,
            MatchResult matchResult) 
            : base(timeline, home, away, matchTime, matchResult) { }

        public override string Content() => $"{HomeTeam.Name} 0 : 0 {AwayTeam.Name}";

        public override string Title() => $"{EmojiConstants.SOUND_ICON} Match Started";
    }
}
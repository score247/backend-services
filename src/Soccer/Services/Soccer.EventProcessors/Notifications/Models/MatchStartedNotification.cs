﻿using Soccer.Core._Shared.Resources;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.Core.Teams.Models;

namespace Soccer.EventProcessors.Notifications.Models
{
    public class MatchStartedNotification : TimelineNotification
    {
        private const string NotificationMatchStart = "NotificationMatchStart";

        public MatchStartedNotification(
            ILanguageResourcesService languageResources,
            TimelineEvent timeline,
            Team home,
            Team away,
            byte matchTime,
            MatchResult matchResult)
            : base(languageResources, timeline, home, away, matchTime, matchResult) { }

        public override string Content(string language = Language.English)
            => $"{HomeTeam.Name} 0{TeamSeparator}0 {AwayTeam.Name}";

        public override string Title(string language = Language.English)
            => string.Format(LanguageResources.GetString(NotificationMatchStart, language));
    }
}
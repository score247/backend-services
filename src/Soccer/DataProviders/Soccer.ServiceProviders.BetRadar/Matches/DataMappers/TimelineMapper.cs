namespace Soccer.DataProviders.SportRadar.Matches.DataMappers
{
    using System.Linq;
    using Score247.Shared.Enumerations;
    using Soccer.Core._Shared.Enumerations;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Teams.Models;

    public static class TimelineMapper
    {
        public static Timeline MapTimeline(Dtos.TimelineDto timelineDto)
        {
            var timeline = new Timeline();

            if (timelineDto != null)
            {
                timeline.Id = timelineDto.id.ToString();
                timeline.Type = Enumeration.FromDisplayName<EventType>(timelineDto.type);
                timeline.Team = timelineDto.team;
                timeline.InjuryTimeAnnounced = timelineDto.injury_time_announced;
                timeline.PeriodType = Enumeration.FromDisplayName<PeriodType>(timelineDto.period_type);
                if (string.IsNullOrWhiteSpace(timeline.PeriodType.DisplayName) && !string.IsNullOrWhiteSpace(timelineDto.period_name))
                {
                    timeline.PeriodType = Enumeration.FromDisplayName<PeriodType>(timelineDto.period_name.ToLowerInvariant().Replace(" ", "_").Trim());
                }
                timeline.Period = timelineDto.period;
                timeline.Time = timelineDto.time;
                timeline.HomeScore = timelineDto.home_score;
                timeline.AwayScore = timelineDto.away_score;
                timeline.StoppageTime = timelineDto.stoppage_time;
                timeline.Commentaries = timelineDto.commentaries?.Select(x => new Commentary { Text = x.text });

                timeline.MatchTime = timelineDto.match_time == 0
                    ? ParseMatchClock(timelineDto.match_clock)
                    : timelineDto.match_time;

                timeline.GoalScorer = timelineDto.goal_scorer == null
                    ? null
                    : new GoalScorer { Name = timelineDto.goal_scorer.name, Id = timelineDto.goal_scorer.id, Method = timelineDto.goal_scorer.method };

                timeline.Assist = timelineDto.assist == null
                    ? null
                    : new Player { Name = timelineDto.assist.name, Id = timelineDto.assist.id };

                timeline.Player = timelineDto.player == null
                    ? null
                    : new Player { Name = timelineDto.player.name, Id = timelineDto.player.id };

                SetPenaltyInfo(timelineDto, timeline);
            }

            return timeline;
        }

        public static int ParseMatchClock(string matchClock)
             => string.IsNullOrWhiteSpace(matchClock)
                             ? 0
                             : int.Parse(matchClock.Split(':')[0]);

        private static void SetPenaltyInfo(Dtos.TimelineDto timelineDto, Timeline timeline)
        {
            if (timelineDto.period_type == PeriodType.Penalties.DisplayName)
            {
                var isScored = timelineDto.status == "scored";
                var player = timelineDto.player == null
                    ? null
                    : new Player { Name = timelineDto.player.name, Id = timelineDto.player.id };
                if (timelineDto.team == "home")
                {
                    timeline.IsHomeShootoutScored = isScored;
                    timeline.HomeShootoutPlayer = player;
                }
                else
                {
                    timeline.IsAwayShootoutScored = isScored;
                    timeline.AwayShootoutPlayer = player;
                }
                timeline.PenaltyStatus = timelineDto.status;
            }
        }
    }
}
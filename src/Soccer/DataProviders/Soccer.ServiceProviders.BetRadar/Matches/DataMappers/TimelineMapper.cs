﻿namespace Soccer.DataProviders.SportRadar.Matches.DataMappers
{
    using System.Linq;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Shared.Enumerations;
    using Soccer.Core.Teams.Models;
    using Soccer.Core.Timeline.Models;
    using Soccer.DataProviders.SportRadar.Matches.Dtos;

    public static class TimelineMapper
    {
        public static TimelineEvent MapTimeline(TimelineDto timelineDto)
        {
            var timeline = new TimelineEvent();

            if (timelineDto != null)
            {
                timeline.Id = timelineDto.id.ToString();
                timeline.Type = Enumeration.FromDisplayName<EventType>(timelineDto.type);
                timeline.Team = timelineDto.team;
                timeline.InjuryTimeAnnounced = timelineDto.injury_time_announced;

                if (!string.IsNullOrWhiteSpace(timelineDto.period_type))
                {
                    timeline.PeriodType = Enumeration.FromDisplayName<PeriodType>(timelineDto.period_type);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(timelineDto.period_name))
                    {
                        timeline.PeriodType = Enumeration.FromDisplayName<PeriodType>(timelineDto.period_name.ToLowerInvariant().Replace(" ", "_").Trim());
                    }
                }

                timeline.Period = timelineDto.period;
                timeline.Time = timelineDto.time;
                timeline.HomeScore = timelineDto.home_score;
                timeline.AwayScore = timelineDto.away_score;
                timeline.StoppageTime = timelineDto.stoppage_time;

                timeline.MatchTime = (byte)(timelineDto.match_time == 0
                    ? ParseMatchClock(timelineDto.match_clock)
                    : timelineDto.match_time);

                timeline.GoalScorer = GetGoalScorer(timelineDto);
                timeline.Assist = GetGoalAssist(timelineDto);
                timeline.Player = GetPlayer(timelineDto.player);

                SetSubsitutionPlayers(timelineDto, timeline);
                SetPenaltyInfo(timelineDto, timeline);
            }

            return timeline;
        }

        private static GoalScorer GetGoalScorer(TimelineDto timelineDto)
            => timelineDto.goal_scorer == null
                                ? null
                                : new GoalScorer
                                {
                                    Name = timelineDto.goal_scorer.name,
                                    Id = timelineDto.goal_scorer.id,
                                    Method = timelineDto.goal_scorer.method
                                };

        private static Player GetGoalAssist(TimelineDto timelineDto)
            => timelineDto.assist == null
                    ? null
                    : new Player { Name = timelineDto.assist.name, Id = timelineDto.assist.id };

        private static Player GetPlayer(PlayerDto playerDto)
            => playerDto == null
                    ? null
                    : new Player { Name = playerDto.name, Id = playerDto.id };

        public static TimelineCommentary MapTimelineCommentary(TimelineDto timelineDto)
            => timelineDto.commentaries == null
                    ? null
                    : new TimelineCommentary(
                        timelineDto.id,
                        timelineDto.commentaries.Select(x => new Commentary { Text = x.text }).ToList());

        public static int ParseMatchClock(string matchClock)
             => string.IsNullOrWhiteSpace(matchClock)
                             ? 0
                             : int.Parse(matchClock.Split(':')[0]);

        private static void SetPenaltyInfo(TimelineDto timelineDto, TimelineEvent timeline)
        {
            if (timelineDto.period_type == PeriodType.Penalties.DisplayName)
            {
                var isScored = timelineDto.status == "scored";

                var player = GetPlayer(timelineDto.player);

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

        private static void SetSubsitutionPlayers(TimelineDto timelineDto, TimelineEvent timeline) 
        {
            if (timeline.Type == EventType.Substitution)
            {
                timeline.PlayerIn = GetPlayer(timelineDto.player_in);
                timeline.PlayerOut = GetPlayer(timelineDto.player_out);
            }
        }
    }
}
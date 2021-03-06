using System.Linq;
using Score247.Shared.Enumerations;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.Core.Teams.Models;
using Soccer.Core.Timelines.Models;
using Soccer.DataProviders.SportRadar._Shared;
using Soccer.DataProviders.SportRadar.Matches.Dtos;

namespace Soccer.DataProviders.SportRadar.Matches.DataMappers
{
    public static class TimelineMapper
    {
        private const string homeTeamIdentifier = "home";
        private const string scoredPenaltyStatus = "scored";

        public static TimelineEvent MapTimeline(TimelineDto timelineDto)
        {
            if (timelineDto == null)
            {
                return null;
            }

            var timeline = new TimelineEvent(
                timelineDto.id.ToString(),
                Enumeration.FromDisplayName<EventType>(timelineDto.type),
                timelineDto.time,
                (byte)(timelineDto.match_time == 0
                    ? ParseMatchClock(timelineDto.match_clock)
                    : timelineDto.match_time),
                timelineDto.stoppage_time,
                timelineDto.team,
                timelineDto.period,
                GetPeriodType(timelineDto),
                timelineDto.home_score,
                timelineDto.away_score,
                GetGoalScorer(timelineDto),
                GetGoalAssist(timelineDto),
                GetPlayer(timelineDto.player),
                timelineDto.injury_time_announced,
                Enumerable.Empty<Commentary>(),
                GetPlayer(timelineDto.player_out),
                GetPlayer(timelineDto.player_in));

            SetPenaltyInfo(timelineDto, timeline);

            return timeline;
        }

        private static PeriodType GetPeriodType(TimelineDto timelineDto)
        {
            if (!string.IsNullOrWhiteSpace(timelineDto.period_type))
            {
                return Enumeration.FromDisplayName<PeriodType>(timelineDto.period_type);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(timelineDto.period_name))
                {
                    return Enumeration
                        .FromDisplayName<PeriodType>(timelineDto.period_name.ToLowerInvariant()
                        .Replace(" ", "_").Trim());
                }
            }

            return new PeriodType();
        }

        private static GoalScorer GetGoalScorer(TimelineDto timelineDto)
            => timelineDto.goal_scorer == null
                                ? null
                                : new GoalScorer(
                                    timelineDto.goal_scorer.id,
                                    PlayerNameConverter.Convert(timelineDto.goal_scorer.name, false),
                                    timelineDto.goal_scorer.method);

        private static Player GetGoalAssist(TimelineDto timelineDto)
            => timelineDto.assist == null
                    ? null
                    : new Player(timelineDto.assist.id, PlayerNameConverter.Convert(timelineDto.assist.name, false));

        private static Player GetPlayer(PlayerDto playerDto)
            => playerDto == null
                ? null
                : new Player(playerDto.id, PlayerNameConverter.Convert(playerDto.name, false));

        public static TimelineCommentary MapTimelineCommentary(TimelineDto timelineDto)
            => timelineDto.commentaries == null
                    ? null
                    : new TimelineCommentary(
                        timelineDto.id,
                        timelineDto.commentaries.Select(x => new Commentary(x.text)).ToList());

        public static int ParseMatchClock(string matchClock)
             => string.IsNullOrWhiteSpace(matchClock)
                             ? 0
                             : int.Parse(matchClock.Split(':')[0]);

        private static void SetPenaltyInfo(TimelineDto timelineDto, TimelineEvent timeline)
        {
            if (timelineDto.period_type == PeriodType.Penalties.DisplayName)
            {
                var isScored = timelineDto.status == scoredPenaltyStatus;

                var player = GetPlayer(timelineDto.player);

                if (timelineDto.team == homeTeamIdentifier)
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
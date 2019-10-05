namespace Soccer.DataReceivers.ScheduleTasks.Matches
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Hangfire;
    using MassTransit;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Matches.QueueMessages;
    using Soccer.Core.Shared.Enumerations;
    using Soccer.Core.Teams.QueueMessages;
    using Soccer.DataProviders.Matches.Services;

    public interface IFetchTimelineTask
    {
        void FetchTimelines(string matchId, string regionName);
    }

    public class FetchTimelineTask : IFetchTimelineTask
    {
        private readonly ITimelineService timelineService;
        private readonly IBus messageBus;

        public FetchTimelineTask(
            IBus messageBus,
            ITimelineService timelineService)
        {
            this.messageBus = messageBus;
            this.timelineService = timelineService;
        }

        public void FetchTimelines(string matchId, string regionName)
        {
            foreach (var language in Enumeration.GetAll<Language>())
            {
                BackgroundJob.Enqueue(() => FetchTimelines(matchId, regionName, language));
            }
        }

        public async Task FetchTimelines(string matchId, string region, Language language)
        {
            var match = await timelineService.GetTimelines(matchId, region, language);

            if (match.Teams == null)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(match.Referee) || match.Attendance > 0)
            {
                await messageBus.Publish<IMatchUpdatedConditionsMessage>(new MatchUpdatedConditionsMessage(matchId, match.Referee, match.Attendance, language));
            }

            foreach (var team in match.Teams)
            {
                await messageBus.Publish<ITeamStatisticUpdatedMessage>(new TeamStatisticUpdatedMessage(matchId, team.IsHome, team.Statistic));
            }

            if (match.TimeLines != null && match.TimeLines.Any())
            {
                var latestTimeline = match.TimeLines.LastOrDefault();

                if (latestTimeline == null)
                {
                    return;
                }

                var matchEvent = new MatchEvent(match.League.Id, match.Id, match.MatchResult, latestTimeline, true);
                await messageBus.Publish<IMatchEventReceivedMessage>(
                    new MatchEventReceivedMessage(matchEvent.AddScoreToSpecialTimeline(match.MatchResult)));

                await ProcessBreakStartEvent(match, match.TimeLines);

                var filteredTimelinesButLast = match.TimeLines.SkipLast(1).ToList();

                foreach (var timeline in filteredTimelinesButLast)
                {
                    await messageBus.Publish<IMatchEventReceivedMessage>(
                        new MatchEventReceivedMessage(new MatchEvent(
                                match.League.Id,
                                match.Id,
                                match.MatchResult,
                                latestTimeline,
                                false)));
                }
            }

            // TODO: Add update match kick off time
        }

        private async Task ProcessBreakStartEvent(Match match, IEnumerable<TimelineEvent> timelines)
        {
            var breakStarts = timelines.Where(t => t.Type == EventType.BreakStart).ToList();

            foreach (var breakStart in breakStarts)
            {
                var latestScore = timelines
                    .LastOrDefault(t => t.Type == EventType.ScoreChange && t.Time < breakStart.Time);

                breakStart.HomeScore = latestScore == null ? 0 : latestScore.HomeScore;
                breakStart.AwayScore = latestScore == null ? 0 : latestScore.AwayScore;

                await messageBus.Publish<IMatchEventReceivedMessage>(
                        new MatchEventReceivedMessage(new MatchEvent(
                                match.League.Id,
                                match.Id,
                                match.MatchResult,
                                breakStart,
                                false)));
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using MassTransit;
using Soccer.Core.Matches.Models;
using Soccer.Core.Matches.QueueMessages;
using Soccer.Core.Shared.Enumerations;
using Soccer.Core.Teams.Models;
using Soccer.Core.Teams.QueueMessages;
using Soccer.Core.Timelines.Models;
using Soccer.Core.Timelines.QueueMessages;
using Soccer.DataProviders.Matches.Services;

namespace Soccer.DataReceivers.ScheduleTasks.Matches
{
    public interface IFetchTimelineTask
    {
        [AutomaticRetry(Attempts = 1)]
        [Queue("mediumlive")]
        Task FetchTimelineEvents(IEnumerable<Match> matches, Language language);

        [AutomaticRetry(Attempts = 1)]
        [Queue("mediumlive")]
        Task FetchTimelineEvents(string matchId, string region, Language language);

        [AutomaticRetry(Attempts = 1)]
        [Queue("low")]
        Task FetchTimelineEventsForClosedMatch(IEnumerable<Match> matches, Language language);

        [AutomaticRetry(Attempts = 1)]
        [Queue("low")]
        Task PublishTeamStatistic(string matchId, DateTimeOffset eventDate, Team team);
    }

    public class FetchTimelineTask : IFetchTimelineTask
    {
        private static readonly TimeSpan insertStatisticDelayTime = new TimeSpan(0, 0, 60);
        private readonly ITimelineService timelineService;
        private readonly IBackgroundJobClient jobClient;
        private readonly IBus messageBus;

        public FetchTimelineTask(
            IBus messageBus,
            ITimelineService timelineService,
            IBackgroundJobClient jobClient)
        {
            this.messageBus = messageBus;
            this.timelineService = timelineService;
            this.jobClient = jobClient;
        }

        public Task FetchTimelineEventsForClosedMatch(IEnumerable<Match> matches, Language language)
            => FetchTimelineEvents(matches, language);

        public async Task FetchTimelineEvents(IEnumerable<Match> matches, Language language)
        {
            foreach (var match in matches)
            {
                await FetchTimelineEvents(match.Id, match.Region, language);
            }
        }

        public async Task FetchTimelineEvents(string matchId, string region, Language language)
        {
            var matchCommentaries = await timelineService.GetTimelineEvents(matchId, region, language);
            var match = matchCommentaries?.Item1;
            var commentaries = matchCommentaries?.Item2;

            if (match == null || match.Teams == null)
            {
                return;
            }

            await PublishMatchCondition(matchId, language, match);

            await PublishMatchTimelineEvents(language, match);

            await PublishMatchCommentaries(matchId, language, match, commentaries);

            await messageBus.Publish<IMatchUpdatedCoverageInfo>(new MatchUpdatedCoverageInfo(matchId, match.Coverage, match.EventDate));

            PublishMatchStatistic(matchId, match);
        }

        private async Task PublishMatchCondition(string matchId, Language language, Match match)
        {
            if (!string.IsNullOrWhiteSpace(match.Referee) || match.Attendance > 0)
            {
                await messageBus.Publish<IMatchUpdatedConditionsMessage>(new MatchUpdatedConditionsMessage(
                    matchId,
                    match.Referee,
                    match.Attendance,
                    language,
                    match.EventDate));
            }
        }

        private void PublishMatchStatistic(string matchId, Match match)
        {
            foreach (var team in match.Teams.Where(team => team.Statistic != null).Select(team => team))
            {
                jobClient.Schedule<IFetchTimelineTask>(
                    t => t.PublishTeamStatistic(matchId, match.EventDate, team), insertStatisticDelayTime);
            }
        }

        public async Task PublishTeamStatistic(string matchId, DateTimeOffset eventDate, Team team)
            => await messageBus.Publish<ITeamStatisticUpdatedMessage>(
                                new TeamStatisticUpdatedMessage(
                                    matchId,
                                    team.IsHome,
                                    team.Statistic,
                                    false,
                                    eventDate));

        private async Task PublishMatchTimelineEvents(Language language, Match match)
        {
            if (match.TimeLines?.Any() == true)
            {
                await messageBus.Publish<IMatchTimelinesFetchedMessage>(new MatchTimelinesFetchedMessage(
                    match,
                    language));
            }
        }

        private async Task PublishMatchCommentaries(string matchId, Language language, Match match, IEnumerable<TimelineCommentary> commentaries)
        {
            if (commentaries != null)
            {
                foreach (var commentary in from commentary in commentaries
                                           where commentary.Commentaries.Count > 0
                                           select commentary)
                {
                    await messageBus.Publish<IMatchCommentaryFetchedMessage>(
                        new MatchCommentaryFetchedMessage(match.League.Id, matchId, commentary, language, match.EventDate));
                }
            }
        }
    }
}
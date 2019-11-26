using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using MassTransit;
using Soccer.Core.Matches.Models;
using Soccer.Core.Matches.QueueMessages;
using Soccer.Core.Shared.Enumerations;
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
        Task FetchTimelines(IEnumerable<Match> matches, Language language);

        [AutomaticRetry(Attempts = 1)]
        [Queue("mediumlive")]
        Task FetchTimelines(string matchId, string region, Language language);

        [AutomaticRetry(Attempts = 1)]
        [Queue("low")]
        Task FetchTimelinesForClosedMatch(IEnumerable<Match> matches, Language language);
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

        public Task FetchTimelinesForClosedMatch(IEnumerable<Match> matches, Language language)
        => FetchTimelines(matches, language);

        public async Task FetchTimelines(IEnumerable<Match> matches, Language language)
        {
            foreach (var match in matches)
            {
                await FetchTimelines(match.Id, match.Region, language);
            }
        }

        public async Task FetchTimelines(string matchId, string region, Language language)
        {
            var matchCommentaries = await timelineService.GetTimelines(matchId, region, language);
            var match = matchCommentaries?.Item1;
            var commentaries = matchCommentaries?.Item2;

            if (match == null || match.Teams == null)
            {
                return;
            }

            await PubishMatchCondition(matchId, language, match);

            await PublishTeamStatistic(matchId, match);

            await PublishMatchTimelines(language, match);

            await PublishMatchCommentaries(matchId, language, match, commentaries);

            await messageBus.Publish<IMatchUpdatedCoverageInfo>(new MatchUpdatedCoverageInfo(matchId, match.Coverage));
        }

        private async Task PubishMatchCondition(string matchId, Language language, Match match)
        {
            if (!string.IsNullOrWhiteSpace(match.Referee) || match.Attendance > 0)
            {
                await messageBus.Publish<IMatchUpdatedConditionsMessage>(new MatchUpdatedConditionsMessage(matchId, match.Referee, match.Attendance, language));
            }
        }

        private async Task PublishTeamStatistic(string matchId, Match match)
        {
            foreach (var team in match.Teams.Where(team => team.Statistic != null).Select(team => team))
            {
                await messageBus.Publish<ITeamStatisticUpdatedMessage>(
                    new TeamStatisticUpdatedMessage(matchId, team.IsHome, team.Statistic));
            }
        }

        private async Task PublishMatchTimelines(Language language, Match match)
        {
            if (match.TimeLines != null && match.TimeLines.Any())
            {
                await messageBus.Publish<IMatchTimelinesFetchedMessage>(new MatchTimelinesFetchedMessage(match, language));
            }
        }

        private async Task PublishMatchCommentaries(string matchId, Language language, Match match, IEnumerable<TimelineCommentary> commentaries)
        {
            if (commentaries != null)
            {
                foreach (var commentary in from commentary in commentaries
                                           where commentary.Commentaries.Any()
                                           select commentary)
                {
                    await messageBus.Publish<IMatchCommentaryFetchedMessage>(
                        new MatchCommentaryFetchedMessage(match.League.Id, matchId, commentary, language));
                }
            }
        }
    }
}
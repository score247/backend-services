using Soccer.Core.Timelines.Models;
using Soccer.Core.Timelines.QueueMessages;

namespace Soccer.DataReceivers.ScheduleTasks.Matches
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Hangfire;
    using MassTransit;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Matches.QueueMessages;
    using Soccer.Core.Shared.Enumerations;
    using Soccer.Core.Teams.QueueMessages;
    using Soccer.DataProviders.Matches.Services;

    public interface IFetchTimelineTask
    {
        [Queue("medium")]
        Task FetchTimelines(string matchId, string regionName);

        [Queue("medium")]
        Task FetchTimelines(string matchId, string region, Language language);
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

        public async Task FetchTimelines(string matchId, string regionName)
        {
            foreach (var language in Enumeration.GetAll<Language>())
            {
                await FetchTimelines(matchId, regionName, language);
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

            if (!string.IsNullOrWhiteSpace(match.Referee) || match.Attendance > 0)
            {
                await messageBus.Publish<IMatchUpdatedConditionsMessage>(new MatchUpdatedConditionsMessage(matchId, match.Referee, match.Attendance, language));
            }

            await PublishTeamStatistic(matchId, match);

            if (match.TimeLines != null && match.TimeLines.Any())
            {
                await messageBus.Publish<IMatchTimelinesFetchedMessage>(new MatchTimelinesFetchedMessage(match, language));
            }

            if (commentaries != null)
            {
                await PublishCommentaries(matchId, match.League.Id, language, commentaries);
            }
        }

        private async Task PublishTeamStatistic(string matchId, Core.Matches.Models.Match match)
        {
            foreach (var team in match.Teams.Where(team => team.Statistic != null).Select(team => team))
            {
                await messageBus.Publish<ITeamStatisticUpdatedMessage>(
                    new TeamStatisticUpdatedMessage(matchId, team.IsHome, team.Statistic));
            }
        }

        private async Task PublishCommentaries(string matchId, string leagueId, Language language, IEnumerable<TimelineCommentary> commentaries)
        {
            foreach (var commentary in from commentary in commentaries
                                       where commentary.Commentaries.Any()
                                       select commentary)
            {
                await messageBus.Publish<IMatchCommentaryFetchedMessage>(
                    new MatchCommentaryFetchedMessage(leagueId, matchId, commentary, language));
            }
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using MassTransit;
using Soccer.Core.Matches.Models;
using Soccer.Core.Matches.QueueMessages;
using Soccer.Core.Shared.Enumerations;
using Soccer.DataProviders.Matches.Services;

namespace Soccer.DataReceivers.ScheduleTasks.Matches
{
    public interface IFetchPreMatchesTimelineTask
    {
        [AutomaticRetry(Attempts = 1)]
        [Queue("medium")]
        Task FetchPreMatchTimeline(IList<Match> matches);
    }

    public class FetchPreMatchesTimelineTask : IFetchPreMatchesTimelineTask
    {
        private readonly ITimelineService timelineService;
        private readonly IBus messageBus;

        public FetchPreMatchesTimelineTask(
            IBus messageBus,
            ITimelineService timelineService)
        {
            this.messageBus = messageBus;
            this.timelineService = timelineService;
        }

        public async Task FetchPreMatchTimeline(IList<Match> matches)
        {
            if (matches == null || !matches.Any())
            {
                return;
            }

            foreach (var match in matches)
            {
                var matchCommentaries = await timelineService.GetTimelines(match.Id, match.Region, Language.en_US);

                if (matchCommentaries?.Item1?.Teams == null || matchCommentaries.Item1?.Coverage == null)
                {
                    continue;
                }

                await messageBus.Publish<IMatchUpdatedCoverageInfo>(new MatchUpdatedCoverageInfo(match.Id, matchCommentaries.Item1.Coverage));
            }
        }
    }
}
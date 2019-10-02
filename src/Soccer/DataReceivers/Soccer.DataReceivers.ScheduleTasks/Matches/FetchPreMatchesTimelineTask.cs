using System.Collections.Generic;
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
        void FetchPreMatchTimeline(IList<Match> matches);       
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

        public void FetchPreMatchTimeline(IList<Match> matches)
        {
            foreach (var match in matches)
            {
                BackgroundJob.Enqueue<IFetchTimelineTask>(t => t.FetchTimelines(match.Id, match.Region));
            }
        }

        public async Task FetchTimelines(string matchId, string region, Language language)
        {
            var match = await timelineService.GetTimelines(matchId, region, language);

            if (match.Teams == null)
            {
                return;
            }
           
            if (match.Coverage != null)
            {
                await messageBus.Publish<IMatchUpdatedCoverageInfo>(new MatchUpdatedCoverageInfo(matchId, language, match.Coverage));
            }
        }
    }
}

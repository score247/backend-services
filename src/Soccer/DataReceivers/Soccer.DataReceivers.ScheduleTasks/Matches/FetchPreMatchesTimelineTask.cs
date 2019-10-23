﻿using System.Collections.Generic;
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
        [Queue("medium")]
        void FetchPreMatchTimeline(IList<Match> matches);

        [Queue("medium")]
        Task FetchPreMatchTimeline(string matchId, string region);
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
                BackgroundJob.Enqueue<IFetchPreMatchesTimelineTask>(t => t.FetchPreMatchTimeline(match.Id, match.Region));
            }
        }

        public async Task FetchPreMatchTimeline(string matchId, string region)
        {
            //Note: since we only need coverage_info which does not have language
            var match = await timelineService.GetTimelines(matchId, region, Language.en_US);

            if (match?.Teams == null)
            {
                return;
            }

            if (match.Coverage != null)
            {
                await messageBus.Publish<IMatchUpdatedCoverageInfo>(new MatchUpdatedCoverageInfo(matchId, match.Coverage));
            }
        }
    }
}
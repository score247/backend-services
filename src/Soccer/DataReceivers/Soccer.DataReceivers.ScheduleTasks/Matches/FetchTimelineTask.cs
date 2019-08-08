namespace Soccer.DataReceivers.ScheduleTasks.Matches
{
    using System.Threading.Tasks;
    using Hangfire;
    using MassTransit;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Matches.Extensions;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Matches.QueueMessages;
    using Soccer.Core.Matches.QueueMessages.MatchEvents;
    using Soccer.Core.Shared.Enumerations;
    using Soccer.DataProviders.Matches.Services;

    public interface IFetchTimelineTask
    {
        void FetchTimelines(string matchId, string regionName);

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

            //if (match.TimeLines != null)
            //{
            //    foreach (var timelineItem in match.TimeLines)
            //    {
            //        var matchEvent = new MatchEvent(match.Id, match.MatchResult, timelineItem);

            //        if (timelineItem.IsScoreChangeInPenalty())
            //        {
            //            return;
            //        }

            //        if (timelineItem.IsShootOutInPenalty())
            //        {
            //            await messageBus.Publish<IPenaltyEventReceivedMessage>(new PenaltyEventReceivedMessage(matchEvent));
            //            return;
            //        }

            //        if (timelineItem.Type.IsMatchEnd())
            //        {
            //            await messageBus.Publish<IMatchEndEventReceivedMessage>(new MatchEndEventReceivedMessage(matchEvent));
            //            return;
            //        }

            //        await messageBus.Publish<INormalEventReceivedMessage>(new NormalEventReceivedMessage(matchEvent));
            //    }
            //}            

            await messageBus.Publish<IMatchUpdatedConditionsMessage>(new MatchUpdatedConditionsMessage(matchId, match.Referee, match.Attendance, language));
        }
    }
}

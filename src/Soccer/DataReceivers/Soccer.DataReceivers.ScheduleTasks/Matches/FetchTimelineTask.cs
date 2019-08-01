namespace Soccer.DataReceivers.ScheduleTasks.Matches
{
    using Hangfire;
    using MassTransit;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Shared.Enumerations;
    using Soccer.DataProviders.Matches.Services;
    using System.Threading.Tasks;

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
            var timelines = await timelineService.GetTimelines(matchId, region, language);

            //TODO publish timeline events
        }
    }
}

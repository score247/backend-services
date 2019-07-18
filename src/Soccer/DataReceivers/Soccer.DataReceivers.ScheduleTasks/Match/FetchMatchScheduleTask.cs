namespace Soccer.DataReceivers.ScheduleTasks.Match
{
    using System;
    using System.Threading.Tasks;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Enumerations;
    using Soccer.DataProviders.Matches.Services;

    public interface IFetchMatchScheduleTask
    {
        Task FetchSchedule(int dateSpan);
    }

    public class FetchMatchScheduleTask : IFetchMatchScheduleTask
    {
        private readonly IMatchService matchService;

        public FetchMatchScheduleTask(IMatchService matchService)
        {
            this.matchService = matchService;
        }

        public async Task FetchSchedule(int dateSpan)
        {
            var from = DateTime.Now;
            var to = DateTime.Now.AddDays(dateSpan);

            foreach (var language in Enumeration.GetAll<Language>())
            {
                await FetchDailySchedule(from, to, language);
            }
        }

        private async Task FetchDailySchedule(DateTime from, DateTime to, Language language)
        {
            var matches = await matchService.GetSchedule(
                   from.ToUniversalTime(),
                   to.ToUniversalTime(),
                   language.Value);
        }
    }
}
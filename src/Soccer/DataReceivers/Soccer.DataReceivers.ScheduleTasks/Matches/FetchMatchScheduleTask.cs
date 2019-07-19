namespace Soccer.DataReceivers.ScheduleTasks.Matches
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MassTransit;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Domain.Matches.Models;
    using Soccer.Core.Enumerations;
    using Soccer.DataProviders.Matches.Services;

    public interface IFetchMatchScheduleTask
    {
        Task FetchSchedule(int dateSpan);
    }

    public class FetchMatchScheduleTask : IFetchMatchScheduleTask
    {
        private readonly IMatchService matchService;
        private readonly IBus messageBus;

        public FetchMatchScheduleTask(IBus messageBus, IMatchService matchService)
        {
            this.messageBus = messageBus;
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
            try
            {
                var matches = await matchService.GetSchedule(
                from.ToUniversalTime(),
                to.ToUniversalTime(),
                language.Value);

                var request = messageBus.CreateRequestClient<FetchScheduleEvent>();

                request.Create(matches);
                var response = await request.GetResponse<string>(new FetchScheduleEvent { Matches = matches });
            }
            catch (Exception ex)
            {
            }
        }
    }

    public class FetchScheduleEvent
    {
        public IEnumerable<Match> Matches { get; set; }
    }
}
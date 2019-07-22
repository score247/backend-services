namespace Soccer.DataReceivers.ScheduleTasks.Matches
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Domain.Matches.Events;
    using Soccer.Core.Enumerations;
    using Soccer.DataProviders.Matches.Services;

    public interface IFetchPostMatchesTask
    {
        Task FetchPostMatches(int dateSpan);
    }

    public class FetchPostMatchesTask : IFetchPostMatchesTask
    {
        private readonly IMatchService matchService;
        private readonly IBus messageBus;

        public FetchPostMatchesTask(IBus messageBus, IMatchService matchService)
        {
            this.messageBus = messageBus;
            this.matchService = matchService;
        }

        public async Task FetchPostMatches(int dateSpan)
        {
            var from = DateTime.UtcNow.AddDays(-dateSpan);
            var to = DateTime.UtcNow;

            foreach (var language in Enumeration.GetAll<Language>())
            {
                await FetchPostMatches(from, to, language);
            }
        }

        private async Task FetchPostMatches(DateTime from, DateTime to, Language language)
        {
            var matches = await matchService.GetPostMatches(
                from,
                to,
                language.Value);

            await messageBus.Publish<PostMatchUpdatedEvent>(new { Matches = matches, Language = language.Value });
        }
    }
}
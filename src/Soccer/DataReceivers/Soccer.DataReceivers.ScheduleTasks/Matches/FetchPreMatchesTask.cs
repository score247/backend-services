﻿namespace Soccer.DataReceivers.ScheduleTasks.Matches
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;
    using Score247.Shared.Enumerations;
    using Soccer.Core._Shared.Enumerations;
    using Soccer.Core.Matches.Events;
    using Soccer.DataProviders.Matches.Services;

    public interface IFetchPreMatchesTask
    {
        Task FetchPreMatches(int dateSpan);
    }

    public class FetchPreMatchesTask : IFetchPreMatchesTask
    {
        private readonly IMatchService matchService;
        private readonly IBus messageBus;

        public FetchPreMatchesTask(IBus messageBus, IMatchService matchService)
        {
            this.messageBus = messageBus;
            this.matchService = matchService;
        }

        public async Task FetchPreMatches(int dateSpan)
        {
            var from = DateTime.UtcNow;
            var to = DateTime.UtcNow.AddDays(dateSpan);

            foreach (var language in Enumeration.GetAll<Language>())
            {
                await FetchPreMatches(from, to, language);
            }
        }

        private async Task FetchPreMatches(DateTime from, DateTime to, Language language)
        {
            var matches = await matchService.GetPreMatches(from, to, language);

            await messageBus.Publish<IPreMatchesFetchedEvent>(new { Matches = matches, Language = language.DisplayName });
        }
    }
}
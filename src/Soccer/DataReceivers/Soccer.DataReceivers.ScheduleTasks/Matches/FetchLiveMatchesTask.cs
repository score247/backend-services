﻿namespace Soccer.DataReceivers.ScheduleTasks.Matches
{
    using MassTransit;
    using Soccer.Core.Matches.Events;
    using Soccer.Core.Shared.Enumerations;
    using Soccer.DataProviders.Matches.Services;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IFetchLiveMatchesTask
    {
        Task FetchLiveMatches();
    }

    public class FetchLiveMatchesTask : IFetchLiveMatchesTask
    {
        private readonly IMatchService matchService;
        private readonly IBus messageBus;

        public FetchLiveMatchesTask(
            IBus messageBus,
            IMatchService matchService)
        {
            this.messageBus = messageBus;
            this.matchService = matchService;
        }

        public async Task FetchLiveMatches()
        {
            var matches = await matchService.GetLiveMatches(Language.en_US);

            await messageBus.Publish<ILiveMatchResultUpdatedMessage>(new LiveMatchResultUpdatedMessage(matches));
        }
    }
}
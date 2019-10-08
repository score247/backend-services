using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using Fanex.Logging;
using MassTransit;
using Soccer.Core.Matches.Events;
using Soccer.Core.Matches.Models;
using Soccer.Core.Matches.QueueMessages;
using Soccer.Core.Shared.Enumerations;
using Soccer.Database.Matches.Commands;
using Soccer.Database.Matches.Criteria;
using Soccer.EventProcessors._Shared.Filters;
using Soccer.EventProcessors.Leagues;

namespace Soccer.EventProcessors.Matches
{
    public class FetchedLiveMatchConsumer : IConsumer<ILiveMatchFetchedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly IBus messageBus;
        private readonly IFilter<IEnumerable<Match>, IEnumerable<Match>> leagueFilter;
        private readonly IFilter<IEnumerable<Match>, IEnumerable<Match>> eventDateFilter;
        private readonly ILeagueGenerator leagueGenerator;
        private readonly ILogger logger;

        public FetchedLiveMatchConsumer(
            IBus messageBus,
            IDynamicRepository dynamicRepository,
            IFilter<IEnumerable<Match>, IEnumerable<Match>> leagueFilter,
            IFilter<IEnumerable<Match>, IEnumerable<Match>> eventDateFilter,
            ILeagueGenerator leagueGenerator,
            ILogger logger)
        {
            this.messageBus = messageBus;
            this.dynamicRepository = dynamicRepository;
            this.leagueFilter = leagueFilter;
            this.eventDateFilter = eventDateFilter;
            this.leagueGenerator = leagueGenerator;
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<ILiveMatchFetchedMessage> context)
        {
            var message = context?.Message;

            if (message == null || message.Matches == null || message.Language == null)
            {
                return;
            }

            IEnumerable<Match> filteredMatches = await FilterMatches(message);

            var currentLiveMatches = (await dynamicRepository
                    .FetchAsync<Match>(new GetLiveMatchesCriteria(message.Language)))
                    .ToList();

            var removedMatches = currentLiveMatches.Except(filteredMatches).ToList();
            var newLiveMatches = filteredMatches.Except(currentLiveMatches).ToList();

            if (removedMatches.Count > 0 || newLiveMatches.Count > 0)
            {
                var tasks = new List<Task>
                {
                    InsertOrRemoveLiveMatches(message.Language, newLiveMatches, removedMatches),
                    PublishLiveMatchUpdatedMessage(message.Language, newLiveMatches, removedMatches)
                };

                await Task.WhenAll(tasks);
            }
        }

        private async Task<IEnumerable<Match>> FilterMatches(ILiveMatchFetchedMessage message)
        {
            var filteredMatches = await leagueFilter.FilterAsync(message.Matches);

            filteredMatches = (await eventDateFilter.FilterAsync(filteredMatches))
                .Select(match => leagueGenerator.GenerateInternationalCode(match)).ToList();

            return filteredMatches;
        }

        private async Task InsertOrRemoveLiveMatches(Language language, IEnumerable<Match> newLiveMatches, IEnumerable<Match> removedMatches)
        {
            try
            {
                var command = new InsertOrRemoveLiveMatchesCommand(language, newLiveMatches, removedMatches);

                await dynamicRepository.ExecuteAsync(command);
            }
            catch (Exception ex)
            {
                await logger.ErrorAsync(ex.Message, ex);
            }
        }

        private Task PublishLiveMatchUpdatedMessage(Language language, IEnumerable<Match> newLiveMatches, IEnumerable<Match> removedMatches)
        => messageBus.Publish<ILiveMatchUpdatedMessage>(new LiveMatchUpdatedMessage(language, newLiveMatches, removedMatches));
    }
}
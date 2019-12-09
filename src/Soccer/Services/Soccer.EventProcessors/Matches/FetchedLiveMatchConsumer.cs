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
using Soccer.EventProcessors.Matches.Filters;

namespace Soccer.EventProcessors.Matches
{
    public class FetchedLiveMatchConsumer : IConsumer<ILiveMatchFetchedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly IBus messageBus;
        private readonly ILiveMatchFilter liveMatchRangeFilter;
        private readonly ILogger logger;

        public FetchedLiveMatchConsumer(
            IBus messageBus,
            IDynamicRepository dynamicRepository,
            ILiveMatchFilter liveMatchFilter,
            ILogger logger)
        {
            this.messageBus = messageBus;
            this.dynamicRepository = dynamicRepository;
            this.liveMatchRangeFilter = liveMatchFilter;
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<ILiveMatchFetchedMessage> context)
        {
            var message = context?.Message;

            if (message?.Matches == null || message.Language == null)
            {
                return;
            }

            var currentLiveMatches = (await dynamicRepository.FetchAsync<Match>(new GetLiveMatchesCriteria(message.Language))).ToList();
            var removedMatches = GetRemovedMatches(message.Matches, currentLiveMatches);
            var newMatches = GetNewMatches(message.Matches, currentLiveMatches).Except(removedMatches).ToList();

            if (removedMatches.Count > 0 || newMatches.Count > 0)
            {
                var tasks = new List<Task>
                {
                    InsertOrRemoveLiveMatches(message.Language, newMatches, removedMatches),
                    PublishLiveMatchUpdatedMessage(message.Language, newMatches, removedMatches)
                };

                await Task.WhenAll(tasks);
            }
        }

        private List<Match> GetNewMatches(IEnumerable<Match> fetchedLiveMatches, IEnumerable<Match> currentLiveMatches)
        {
            var inRangeNotStarted = liveMatchRangeFilter
                .FilterNotStarted(fetchedLiveMatches).ToList();

            // remove out range closed match in api
            var outRangeClosedMatches = GetOutOfRangeClosedMatches(fetchedLiveMatches);

            return inRangeNotStarted
                .Except(currentLiveMatches)
                .Except(outRangeClosedMatches)
                .ToList();
        }

        private IList<Match> GetRemovedMatches(IEnumerable<Match> fetchedLiveMatches, IEnumerable<Match> currentLiveMatches)
        {
            // closed matches were removed from api
            var removedMatches = currentLiveMatches.Except(fetchedLiveMatches).ToList();

            // closed match in db but out of range
            removedMatches.AddRange(GetOutOfRangeClosedMatches(currentLiveMatches));

            // not started match still in api but out of range
            removedMatches.AddRange(GetOutOfRangeNotStartedMatches(currentLiveMatches));

            return removedMatches.Distinct().ToList();
        }

        private IList<Match> GetOutOfRangeClosedMatches(IEnumerable<Match> currentLiveMatches)
        {
            var inRangeClosedMatches = liveMatchRangeFilter.FilterClosed(currentLiveMatches);

            var outOfRangeMatches = (inRangeClosedMatches != null && inRangeClosedMatches.Any()
                ? currentLiveMatches.Except(inRangeClosedMatches)
                : currentLiveMatches.Where(m => m.MatchResult.EventStatus.IsClosed()));

            return outOfRangeMatches.ToList();
        }

        private IList<Match> GetOutOfRangeNotStartedMatches(IEnumerable<Match> currentLiveMatches)
        {
            var inRangeNotStartedMatches = liveMatchRangeFilter
                .FilterNotStarted(currentLiveMatches);

            var outOfRangeMatches = (inRangeNotStartedMatches != null && inRangeNotStartedMatches.Any()
                ? currentLiveMatches.Except(inRangeNotStartedMatches)
                : currentLiveMatches.Where(m => m.MatchResult.EventStatus.IsNotStart()));

            return outOfRangeMatches.ToList();
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
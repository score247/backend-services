using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using Fanex.Logging;
using MassTransit;
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

            if (message?.Matches == null || message.Language == null || message.Regions?.Any() == false)
            {
                await logger.InfoAsync($"FetchLiveMatch - {DateTime.Now} - Consume: Region.Any {message?.Regions?.Any()}");
                return;
            }

            var currentLiveMatches = await GetCurrentLiveMatches(message.Language, message.Regions);

            var removedMatches = GetRemovedMatches(message.Matches, currentLiveMatches);
            var newMatches = GetNewMatches(message.Matches.ToList(), currentLiveMatches).Except(removedMatches).ToList();

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

        private async Task<List<Match>> GetCurrentLiveMatches(Language language, IReadOnlyList<string> regions)
        {
            var liveMatches = (await dynamicRepository.FetchAsync<Match>(new GetLiveMatchesCriteria(language)))
                            ?.Where(match => regions.Contains(match.Region))
                            ?.ToList();

            return liveMatches ?? new List<Match>();
        }

        private IEnumerable<Match> GetNewMatches(IList<Match> fetchedLiveMatches, IEnumerable<Match> currentLiveMatches)
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

        private IList<Match> GetRemovedMatches(IEnumerable<Match> fetchedLiveMatches, IList<Match> currentLiveMatches)
        {
            // closed matches were removed from api
            var removedMatches = currentLiveMatches.Except(fetchedLiveMatches).ToList();

            // closed match in db but out of range
            removedMatches.AddRange(GetOutOfRangeClosedMatches(currentLiveMatches));

            // not started match still in api but out of range
            removedMatches.AddRange(GetOutOfRangeNotStartedMatches(currentLiveMatches));

            return removedMatches.Distinct().ToList();
        }

        private IEnumerable<Match> GetOutOfRangeClosedMatches(IList<Match> currentLiveMatches)
        {
            var inRangeClosedMatches = liveMatchRangeFilter.FilterClosed(currentLiveMatches)?.ToList();

            var outOfRangeMatches = (inRangeClosedMatches?.Count > 0
                ? currentLiveMatches.Except(inRangeClosedMatches)
                : currentLiveMatches.Where(m => m.MatchResult.EventStatus.IsClosed()));

            return outOfRangeMatches;
        }

        private IEnumerable<Match> GetOutOfRangeNotStartedMatches(IList<Match> currentLiveMatches)
        {
            var inRangeNotStartedMatches = liveMatchRangeFilter
                .FilterNotStarted(currentLiveMatches)?.ToList();

            var outOfRangeMatches = (inRangeNotStartedMatches?.Any() == true
                ? currentLiveMatches.Except(inRangeNotStartedMatches)
                : currentLiveMatches.Where(m => m.MatchResult.EventStatus.IsNotStart()));

            return outOfRangeMatches;
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

        private async Task PublishLiveMatchUpdatedMessage(Language language, IEnumerable<Match> newLiveMatches, IEnumerable<Match> removedMatches)
        {
            await messageBus.Publish<ILiveMatchUpdatedMessage>(new LiveMatchUpdatedMessage(language, newLiveMatches, removedMatches));

            if (newLiveMatches?.Count() > 0)
            {
                await logger.InfoAsync($"FetchLiveMatch - {DateTime.Now} - new live matches: {newLiveMatches.Count()}", "Live", newLiveMatches);
            }            
        }
    }
}
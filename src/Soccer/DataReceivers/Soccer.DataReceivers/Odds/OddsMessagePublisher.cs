namespace Soccer.DataReceivers.Odds
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Caching;
    using Fanex.Logging;
    using MassTransit;
    using Newtonsoft.Json;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Odds;
    using Soccer.Core.Odds.Messages;
    using Soccer.Core.Odds.Models;
    using Soccer.DataProviders.Odds;

    public interface IOddsMessagePublisher
    {
        Task PublishOdds(IEnumerable<MatchOdds> oddsList, int batchSize);

        Task PublishOdds(MatchEvent matchEvent);
    }

    public class OddsMessagePublisher : IOddsMessagePublisher
    {
        private static readonly CacheItemOptions MatchEventCacheOption = new CacheItemOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(10),
        };

        private readonly IOddsService oddsService;
        private readonly IBus messageBus;
        private readonly ILogger logger;
        private readonly ICacheService cacheService;

        public OddsMessagePublisher(
            IOddsService oddsService,
            IBus messageBus,
            ILogger logger,
            ICacheService cacheService)
        {
            this.oddsService = oddsService;
            this.messageBus = messageBus;
            this.logger = logger;
            this.cacheService = cacheService;
        }

        public async Task PublishOdds(IEnumerable<MatchOdds> oddsList, int batchSize)
        {
            var total = oddsList.Count();

            for (var batchIndex = 0; batchIndex * batchSize < total; batchIndex++)
            {
                var oddsBatch = oddsList.Skip(batchIndex * batchSize).Take(batchSize);

                await messageBus.Publish<IOddsChangeMessage>(new OddsChangeMessage(oddsBatch));
            }
        }

        public async Task PublishOdds(MatchEvent matchEvent)
        {
            try
            {
                await logger.InfoAsync($"Odds - Match Event: {JsonConvert.SerializeObject(matchEvent)}");

                if (await IsEventBeHandled(matchEvent))
                {
                    return;
                }

                if (matchEvent.MatchResult.EventStatus.IsNotClosedAndNotEnded()
                    && OddsMovementProcessor.IsTimelineNeedMapWithOddsData(matchEvent.Timeline))
                {
                    await GetEventOddsAndPublishOddsMessage(matchEvent);
                }
            }
            catch (Exception ex)
            {
                await logger.ErrorAsync(
                            string.Join(
                            "\r\n",
                            $"Match Event: {JsonConvert.SerializeObject(matchEvent)}",
                            $"Exception: {ex}"),
                            ex);
            }
        }

        private async Task<bool> IsEventBeHandled(MatchEvent matchEvent)
        {
            var cachedMatchEvents = await GetCachedEvents(matchEvent.MatchId);

            return cachedMatchEvents.Any(me => me.Timeline.Id == matchEvent.Timeline.Id);
        }

        private async Task<IList<MatchEvent>> GetCachedEvents(string matchId)
            => (await cacheService.GetAsync<IList<MatchEvent>>(BuildEventsCacheKey(matchId)))
                ?? new List<MatchEvent>();

        private static string BuildEventsCacheKey(string matchId)
            => $"Odds_MatchEvent_{matchId}";

        private async Task GetEventOddsAndPublishOddsMessage(MatchEvent matchEvent)
        {
            var currentOdds = await oddsService.GetOdds(matchEvent.MatchId, matchEvent.Timeline.Time.DateTime);

            await messageBus.Publish<IOddsChangeMessage>(
                new OddsChangeMessage(
                    new List<MatchOdds>
                    {
                        currentOdds
                    },
                    matchEvent));

            await CacheNewEvent(matchEvent);
        }

        private async Task CacheNewEvent(MatchEvent matchEvent)
        {
            var cachedEvents = await GetCachedEvents(matchEvent.MatchId);
            cachedEvents.Add(matchEvent);
            await cacheService.SetAsync(BuildEventsCacheKey(matchEvent.MatchId), cachedEvents, MatchEventCacheOption);
        }
    }
}
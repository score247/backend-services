namespace Soccer.EventProcessors.Matches.MatchEvents
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using MassTransit;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Matches.QueueMessages;
    using Soccer.Core.Matches.QueueMessages.MatchEvents;

    public class ReceivePenaltyEventConsumer : BaseMatchEventConsumer, IConsumer<IPenaltyEventReceivedMessage>
    {
        private static readonly CacheItemOptions MatchPenaltyCacheOptions = new CacheItemOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(10),
        };

        private readonly ICacheService cacheService;
        private readonly IBus messageBus;

        public ReceivePenaltyEventConsumer(IBus messageBus, ICacheService cacheService, IDynamicRepository dynamicRepository)
            : base(cacheService, dynamicRepository)
        {
            this.messageBus = messageBus;
            this.cacheService = cacheService;
        }

        public async Task Consume(ConsumeContext<IPenaltyEventReceivedMessage> context)
        {
            var matchEvent = context?.Message?.MatchEvent;

            if (await EventNotProcessed(matchEvent))
            {
                await HandlePenalty(matchEvent);

                await messageBus.Publish<IMatchEventProcessedMessage>(new MatchEventProcessedMessage(matchEvent));
            }
        }

        public async Task HandlePenalty(MatchEvent matchEvent)
        {
            var matchEventsCacheKey = $"Penalty_Match_{matchEvent.MatchId}";
            var cachedPenaltyEvents = (await cacheService.GetAsync<IList<Timeline>>(matchEventsCacheKey)) ?? new List<Timeline>();

            cachedPenaltyEvents.Add(matchEvent.Timeline);

            await HandleLastShootAndCombineInfoWithCurrentShoot(matchEvent.Timeline, matchEvent.MatchId);

            SetTotalScore(cachedPenaltyEvents, matchEvent.Timeline);

            await CachePenaltyEvents(matchEventsCacheKey, cachedPenaltyEvents);
        }

        private async Task HandleLastShootAndCombineInfoWithCurrentShoot(Timeline shootoutEvent, string matchId)
        {
            var latestEventCacheKey = $"Penalty_Match_{matchId}_Latest_Event";
            var latestShootout = await GetLatestShoot(shootoutEvent, latestEventCacheKey);

            if (latestShootout == null)
            {
                SetFirstShoot(shootoutEvent);
            }

            if (latestShootout != null)
            {
                await HandleLatestPenaltyEvent(latestEventCacheKey, shootoutEvent, latestShootout);
            }
        }

        private async Task<Timeline> GetLatestShoot(Timeline shootoutEvent, string latestEventCacheKey)
        {
            var latestShootout = await cacheService.GetAsync<Timeline>(latestEventCacheKey);

            if (latestShootout == null)
            {
                await cacheService.SetAsync(latestEventCacheKey, shootoutEvent, MatchPenaltyCacheOptions);
            }

            return latestShootout;
        }

        private static void SetFirstShoot(Timeline shootoutEvent)
        {
            shootoutEvent.IsFirstShoot = true;

            if (shootoutEvent.IsHome)
            {
                shootoutEvent.HomeShootoutPlayer = shootoutEvent.Player;
            }
            else
            {
                shootoutEvent.AwayShootoutPlayer = shootoutEvent.Player;
            }
        }

        private async Task HandleLatestPenaltyEvent(
            string latestEventCacheKey,
            Timeline shootoutEvent,
            Timeline latestPenalty)
        {
            if (latestPenalty != null)
            {
                if (shootoutEvent.IsHome && !latestPenalty.IsHome)
                {
                    shootoutEvent.AwayShootoutPlayer = latestPenalty.AwayShootoutPlayer;
                    shootoutEvent.IsAwayShootoutScored = latestPenalty.IsAwayShootoutScored;
                    shootoutEvent.HomeShootoutPlayer = shootoutEvent.Player;
                }
                else
                {
                    shootoutEvent.HomeShootoutPlayer = latestPenalty.HomeShootoutPlayer;
                    shootoutEvent.IsHomeShootoutScored = latestPenalty.IsHomeShootoutScored;
                    shootoutEvent.AwayShootoutPlayer = shootoutEvent.Player;
                }

                await cacheService.RemoveAsync(latestEventCacheKey);
            }
        }

        private async Task CachePenaltyEvents(string cacheKey, IList<Timeline> cachedPenaltyEvents)
            => await cacheService.SetAsync(cacheKey, cachedPenaltyEvents, MatchPenaltyCacheOptions);

        private static void SetTotalScore(IList<Timeline> shootoutEvents, Timeline shootoutTimeline)
        {
            var homeScore = 0;
            var awayScore = 0;

            foreach (var shootoutEvent in shootoutEvents)
            {
                if (shootoutEvent != null)
                {
                    homeScore += shootoutEvent.IsHome && shootoutEvent.IsHomeShootoutScored ? 1 : 0;
                    awayScore += !shootoutEvent.IsHome && shootoutEvent.IsAwayShootoutScored ? 1 : 0;
                }
            }

            shootoutTimeline.ShootoutHomeScore = homeScore;
            shootoutTimeline.ShootoutAwayScore = awayScore;
        }
    }
}
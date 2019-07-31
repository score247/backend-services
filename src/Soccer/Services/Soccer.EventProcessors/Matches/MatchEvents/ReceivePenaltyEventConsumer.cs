﻿namespace Soccer.EventProcessors.Matches.MatchEvents
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

    public class ReceivePenaltyEventConsumer : BaseMatchEventConsumer, IConsumer<IPenaltyEventReceived>
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

        public async Task Consume(ConsumeContext<IPenaltyEventReceived> context)
        {
            var matchEvent = context?.Message?.MatchEvent;

            if (await IsTimelineProcessed(matchEvent))
            {
                await HandlePenalty(matchEvent);

                await messageBus.Publish<IMatchEventProcessed>(new MatchEventProcessed(matchEvent));
            }
        }

        public async Task HandlePenalty(MatchEvent matchEvent)
        {
            var matchEventsCacheKey = $"Penalty_Match_{matchEvent.MatchId}";
            var cachedPenaltyEvents = (await cacheService.GetAsync<IList<TimelineEventEntity>>(matchEventsCacheKey)) ?? new List<TimelineEventEntity>();

            cachedPenaltyEvents.Add(matchEvent.Timeline);

            await HandleLastShootAndCombineInfoWithCurrentShoot(matchEvent.Timeline, matchEvent.MatchId);

            SetTotalScore(cachedPenaltyEvents, matchEvent.Timeline);

            await CachePenaltyEvents(matchEventsCacheKey, cachedPenaltyEvents);
        }

        private async Task HandleLastShootAndCombineInfoWithCurrentShoot(TimelineEventEntity shootoutEvent, string matchId)
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

        private async Task<TimelineEventEntity> GetLatestShoot(TimelineEventEntity shootoutEvent, string latestEventCacheKey)
        {
            var latestShootout = await cacheService.GetAsync<TimelineEventEntity>(latestEventCacheKey);

            if (latestShootout == null)
            {
                await cacheService.SetAsync(latestEventCacheKey, shootoutEvent, MatchPenaltyCacheOptions);
            }

            return latestShootout;
        }

        private static void SetFirstShoot(TimelineEventEntity shootoutEvent)
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
            TimelineEventEntity shootoutEvent,
            TimelineEventEntity latestPenalty)
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

        private async Task CachePenaltyEvents(string cacheKey, IList<TimelineEventEntity> cachedPenaltyEvents)
            => await cacheService.SetAsync(cacheKey, cachedPenaltyEvents, MatchPenaltyCacheOptions);

        private static void SetTotalScore(IList<TimelineEventEntity> shootoutEvents, TimelineEventEntity shootoutTimeline)
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
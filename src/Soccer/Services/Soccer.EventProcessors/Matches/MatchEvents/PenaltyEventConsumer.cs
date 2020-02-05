using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Caching;
using MassTransit;
using Soccer.Core.Matches.Models;
using Soccer.Core.Matches.QueueMessages;
using Soccer.Core.Matches.QueueMessages.MatchEvents;

namespace Soccer.EventProcessors.Matches.MatchEvents
{
    public class PenaltyEventConsumer : IConsumer<IPenaltyEventMessage>
    {
        private const byte DefaultPenaltyMatchTime = 121;

        private static readonly CacheItemOptions MatchPenaltyCacheOptions = new CacheItemOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(10),
        };

        private readonly IBus messageBus;
        private readonly ICacheService cacheService;

        public PenaltyEventConsumer(IBus messageBus, ICacheService cacheService)

        {
            this.messageBus = messageBus;
            this.cacheService = cacheService;
        }

        public async Task Consume(ConsumeContext<IPenaltyEventMessage> context)
        {
            var matchEvent = context?.Message?.MatchEvent;

            if (matchEvent == null)
            {
                return;
            }

            var isCompleted = await HandlePenalty(matchEvent);

            if (isCompleted)
            {
                matchEvent.Timeline.UpdateMatchTime(DefaultPenaltyMatchTime);
                await messageBus.Publish<IMatchEventProcessedMessage>(new MatchEventProcessedMessage(matchEvent));
            }
        }

        public async Task<bool> HandlePenalty(MatchEvent matchEvent)
        {
            var matchEventsCacheKey = $"Penalty_Match_{matchEvent.MatchId}";
            var cachedPenaltyEvents = (await cacheService.GetAsync<IList<TimelineEvent>>(matchEventsCacheKey)) ?? new List<TimelineEvent>();

            if (cachedPenaltyEvents.Any(t => t.Id == matchEvent.Timeline.Id))
            {
                return false;
            }

            cachedPenaltyEvents.Add(matchEvent.Timeline);

            await HandleLastShootAndCombineInfoWithCurrentShoot(matchEvent.Timeline, matchEvent.MatchId);

            SetTotalScore(cachedPenaltyEvents, matchEvent.Timeline);

            await CachePenaltyEvents(matchEventsCacheKey, cachedPenaltyEvents);

            return true;
        }

        private async Task HandleLastShootAndCombineInfoWithCurrentShoot(TimelineEvent shootoutEvent, string matchId)
        {
            var latestEventCacheKey = $"Penalty_Match_{matchId}_Latest_Event";
            var latestShootout = await GetLatestShoot(shootoutEvent, latestEventCacheKey);

            if (latestShootout == null)
            {
                SetFirstShoot(shootoutEvent);
            }
            else
            {
                await HandleLatestPenaltyEvent(latestEventCacheKey, shootoutEvent, latestShootout);
            }
        }

        private async Task<TimelineEvent> GetLatestShoot(TimelineEvent shootoutEvent, string latestEventCacheKey)
        {
            var latestShootout = await cacheService.GetAsync<TimelineEvent>(latestEventCacheKey);

            if (latestShootout == null)
            {
                await cacheService.SetAsync(latestEventCacheKey, shootoutEvent, MatchPenaltyCacheOptions);
            }

            return latestShootout;
        }

        private static void SetFirstShoot(TimelineEvent shootoutEvent)
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
            TimelineEvent shootoutEvent,
            TimelineEvent latestPenalty)
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

        private async Task CachePenaltyEvents(string cacheKey, IList<TimelineEvent> cachedPenaltyEvents)
            => await cacheService.SetAsync(cacheKey, cachedPenaltyEvents, MatchPenaltyCacheOptions);

        private static void SetTotalScore(IList<TimelineEvent> shootoutEvents, TimelineEvent shootoutTimeline)
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
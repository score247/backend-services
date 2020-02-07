using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Caching;
using Fanex.Data.Repository;
using MassTransit;
using Score247.Shared;
using Soccer.Core._Shared.Enumerations;
using Soccer.Core.Matches.Models;
using Soccer.Core.Notification.Models;
using Soccer.Core.Notification.QueueMessages;
using Soccer.Core.Shared.Enumerations;
using Soccer.Database.Favorites.Criteria;
using Soccer.Database.Matches.Criteria;
using Soccer.EventProcessors.Notifications.Models;

namespace Soccer.EventProcessors.Notifications
{
    public class ReceiveMatchNotificationConsumer : IConsumer<IMatchNotificationReceivedMessage>
    {
        private static readonly CacheItemOptions EventCacheOptions = new CacheItemOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(120),
        };

        private readonly IBus messageBus;
        private readonly IDynamicRepository dynamicRepository;
        private readonly ICacheManager cacheManager;

        public ReceiveMatchNotificationConsumer(
            IBus messageBus,
            IDynamicRepository dynamicRepository,
            ICacheManager cacheManager)
        {
            this.messageBus = messageBus;
            this.dynamicRepository = dynamicRepository;
            this.cacheManager = cacheManager;
        }

        public async Task Consume(ConsumeContext<IMatchNotificationReceivedMessage> context)
        {
            var message = context.Message;
            var match = await GetMatchAsync(message.MatchId);
            var userIds = await GetUserFavoriteIdsAsync(message.MatchId);

            if (userIds?.Any() == false || match == null)
            {
                return;
            }

            var notification = TimelineNotificationCreator.CreateInstance(
                message.Timeline.Type,
                message.Timeline,
                match.Teams.FirstOrDefault(team => team.IsHome),
                match.Teams.FirstOrDefault(team => !team.IsHome),
                message.Timeline.MatchTime,
                message.MatchResult);

            if (notification == null)
            {
                return;
            }

            //TODO de-dup message
            var isProcessed = await IsProcessedNotification(message.MatchId, notification.Content());

            if (isProcessed)
            {
                return;
            }

            //TODO language translation
            //TODO queue by batch of users
            //TODO consider user platform

            await messageBus.Publish<IMatchNotificationProcessedMessage>(new MatchNotificationProcessedMessage(
                new MatchEventNotification(
                    message.MatchId,
                    notification.Title(),
                    notification.Content(),
                    userIds: userIds
                )));

            await SetProcessedNotificationAsync(message.MatchId, notification.Content());
        }

        private async Task<string[]> GetUserFavoriteIdsAsync(string matchId)
        => (await dynamicRepository
                           .FetchAsync<string>(new GetUsersByFavoriteIdCriteria(matchId, FavoriteType.Match.Value)))
                           ?.ToArray();

        private async Task<Match> GetMatchAsync(string matchId)
        {
            var matchCacheKey = $"MatchInfo_{matchId}";

            var match = await cacheManager.GetOrSetAsync(
                matchCacheKey,
                async () =>
                {
                    return (await dynamicRepository.GetAsync<Match>(new GetMatchByIdCriteria(matchId, Language.en_US))
                        ?? default);
                },
                EventCacheOptions);

            return match;
        }

        private async Task<bool> IsProcessedNotification(string matchId, string content)
        {
            var matchNotificationCacheKey = $"MatchNotification_{matchId}";
            var processedNotifications = await GetProcessedNotificationsAsync(matchNotificationCacheKey);

            return processedNotifications.Contains(content);
        }

        private async Task<List<string>> GetProcessedNotificationsAsync(string matchNotificationCacheKey)
        {
            return await cacheManager.GetOrSetAsync(
                            matchNotificationCacheKey,
                            async () =>
                            {
                                return await Task.FromResult(new List<string>());
                            },
                            EventCacheOptions);
        }

        private async Task SetProcessedNotificationAsync(string matchId, string content)
        {
            var matchNotificationCacheKey = $"MatchNotification_{matchId}";

            var processedNotifications = await GetProcessedNotificationsAsync(matchNotificationCacheKey);
            processedNotifications.Add(content);
        }
    }
}

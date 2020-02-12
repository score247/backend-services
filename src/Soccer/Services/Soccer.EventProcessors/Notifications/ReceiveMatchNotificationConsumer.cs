using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Caching;
using Fanex.Data.Repository;
using Fanex.Logging;
using MassTransit;
using Score247.Shared;
using Score247.Shared.Constants;
using Soccer.Core._Shared.Resources;
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
        private static readonly CacheItemOptions NotificationCacheOptions = new CacheItemOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(360), //Max minutes of a match
        };

        private readonly IBus messageBus;
        private readonly IDynamicRepository dynamicRepository;
        private readonly ICacheManager cacheManager;
        private readonly ILogger logger;
        private readonly ILanguageResourcesService languageResources;

        public ReceiveMatchNotificationConsumer(
            IBus messageBus,
            IDynamicRepository dynamicRepository,
            ICacheManager cacheManager,
            ILogger logger,
            ILanguageResourcesService languageResources)
        {
            this.messageBus = messageBus;
            this.dynamicRepository = dynamicRepository;
            this.cacheManager = cacheManager;
            this.logger = logger;
            this.languageResources = languageResources;
        }

        public async Task Consume(ConsumeContext<IMatchNotificationReceivedMessage> context)
        {
            var message = context.Message;
            var favoriteUserIds = await GetUserFavoriteIdsAsync(message.MatchId);
            var notification = await GenerateTimelineNotificationAsync(message);

            if (notification == null || favoriteUserIds?.Any() == false)
            {
                return;
            }
           
            //TODO queue by batch of users
            //TODO consider user platform
            //TODO get users by language

            await messageBus.Publish<IMatchNotificationProcessedMessage>(new MatchNotificationProcessedMessage(
                new MatchEventNotification(
                    message.MatchId,
                    notification.Title(),
                    notification.Content(),
                    userIds: favoriteUserIds
                )));

            await SetProcessedNotificationAsync(message.MatchId, notification.Content());
        }

        private async Task<TimelineNotification> GenerateTimelineNotificationAsync(IMatchNotificationReceivedMessage message) 
        {
            var match = await GetAndCacheMatchAsync(message.MatchId);

            if (match == null)
            {
                await logger.InfoAsync($"ReceiveMatchNotificationConsumer NotFound match {message.MatchId}");
                return default;
            }

            var notification = TimelineNotificationCreator.CreateInstance(
                languageResources,
                message.Timeline.Type,
                message.Timeline,
                match.Teams.FirstOrDefault(team => team.IsHome),
                match.Teams.FirstOrDefault(team => !team.IsHome),
                message.Timeline.MatchTime,
                message.MatchResult);
                       
            var isProcessed = await IsProcessedNotificationAsync(message.MatchId, notification.Content());

            return isProcessed 
                ? default 
                : notification;
        }

        private async Task<string[]> GetUserFavoriteIdsAsync(string matchId)
        => (await dynamicRepository
                           .FetchAsync<string>(new GetUsersByFavoriteIdCriteria(matchId)))
                           ?.ToArray();

        private async Task<Match> GetAndCacheMatchAsync(string matchId)
        {
            var matchCacheKey = $"{CacheKeys.MATCH_INFO_CACHE_KEY}_{matchId}";

            var match = await cacheManager.GetOrSetAsync(
                matchCacheKey,
                async () =>
                {
                    return await dynamicRepository.GetAsync<Match>(new GetMatchByIdCriteria(matchId, Language.en_US));
                },
                NotificationCacheOptions);

            return match;
        }

        private async Task<bool> IsProcessedNotificationAsync(string matchId, string content)
        {
            var matchNotificationCacheKey = $"{CacheKeys.MATCH_NOTIFICATION_CACHE_KEY}_{matchId}";
            var processedNotifications = await GetProcessedNotificationsAsync(matchNotificationCacheKey);

            return processedNotifications.Contains(content);
        }

        private async Task SetProcessedNotificationAsync(string matchId, string content)
        {
            var matchNotificationCacheKey = $"{CacheKeys.MATCH_NOTIFICATION_CACHE_KEY}_{matchId}";

            var processedNotifications = await GetProcessedNotificationsAsync(matchNotificationCacheKey);
            processedNotifications.Add(content);
        }

        private async Task<BlockingCollection<string>> GetProcessedNotificationsAsync(string matchNotificationCacheKey)
        {
            return await cacheManager.GetOrSetAsync(
                            matchNotificationCacheKey,
                            async () =>
                            {
                                return await Task.FromResult(new BlockingCollection<string>());
                            },
                            NotificationCacheOptions);
        }
    }
}

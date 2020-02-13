using System;
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
using Soccer.EventProcessors.Shared.Configurations;

namespace Soccer.EventProcessors.Notifications
{
    public class ReceiveMatchNotificationConsumer : IConsumer<IMatchNotificationReceivedMessage>
    {
        private static readonly CacheItemOptions MatchInfoCacheOptions = new CacheItemOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(360), //Max minutes of a match
        };

        private readonly IBus messageBus;
        private readonly IDynamicRepository dynamicRepository;
        private readonly ICacheManager cacheManager;
        private readonly ILogger logger;
        private readonly ILanguageResourcesService languageResources;
        private readonly IAppSettings appSettings;
        private readonly IMatchNotificationDeduper notificationDeduper;

        public ReceiveMatchNotificationConsumer(
            IBus messageBus,
            IDynamicRepository dynamicRepository,
            ICacheManager cacheManager,
            ILogger logger,
            ILanguageResourcesService languageResources,
            IAppSettings appSettings,
            IMatchNotificationDeduper notificationDeduper)
        {
            this.messageBus = messageBus;
            this.dynamicRepository = dynamicRepository;
            this.cacheManager = cacheManager;
            this.logger = logger;
            this.languageResources = languageResources;
            this.appSettings = appSettings;
            this.notificationDeduper = notificationDeduper;
        }

        public async Task Consume(ConsumeContext<IMatchNotificationReceivedMessage> context)
        {
            var message = context.Message;

            //TODO group users by language and OS (iOS | Android)
            var favoriteUserIds = await GetUserFavoriteIdsAsync(message.MatchId);
            
            if (favoriteUserIds?.Any() == false)
            {
                return;
            }

            var notification = await GenerateTimelineNotificationAsync(message);

            if (notification == null)
            {
                await logger.InfoAsync($"ReceiveMatchNotificationConsumer NotSupported {message.Timeline.Type.DisplayName}");

                return;
            }

            if (await notificationDeduper.IsProcessedAsync(message.MatchId, notification.Content()))
            {
                return;
            }
            
            for (var i = 0; i * appSettings.MaxUsersSent < favoriteUserIds.Count(); i++)
            {
                var batchOfUsers = favoriteUserIds
                    .Skip(i * appSettings.MaxUsersSent)
                    .Take(appSettings.MaxUsersSent)
                    .ToArray();

                await messageBus.Publish<IMatchNotificationProcessedMessage>(new MatchNotificationProcessedMessage(
                   new MatchEventNotification(
                       message.MatchId,
                       notification.Title(),
                       notification.Content(),
                       userIds: batchOfUsers
                   )));
            }
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

            return notification;
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
                MatchInfoCacheOptions);

            return match;
        }
    }
}

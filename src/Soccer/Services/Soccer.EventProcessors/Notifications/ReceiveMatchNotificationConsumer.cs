using System;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Caching;
using Fanex.Data.Repository;
using Fanex.Logging;
using MassTransit;
using Score247.Shared;
using Score247.Shared.Enumerations;
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
        private readonly ILogger logger;

        public ReceiveMatchNotificationConsumer(
            IBus messageBus,
            IDynamicRepository dynamicRepository,
            ICacheManager cacheManager,
            ILogger logger) 
        {
            this.messageBus = messageBus;
            this.dynamicRepository = dynamicRepository;
            this.cacheManager = cacheManager;
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<IMatchNotificationReceivedMessage> context)
        {
            var message = context.Message;
            var match = await GetMatchAsync(message.MatchId);

            var userIds = (await dynamicRepository
               .FetchAsync<string>(new GetUsersByFavoriteIdCriteria(message.MatchId, FavoriteType.Match.Value)))
               ?.ToArray();

            if (userIds?.Any() == false) 
            {
                return;
            }

            if (match == null)
            {
                await logger.InfoAsync($"ReceiveMatchNotificationConsumer match {message.MatchId} not found");
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
        }

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
    }
}

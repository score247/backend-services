using System;
using System.Threading.Tasks;
using AutoFixture;
using Fanex.Caching;
using Fanex.Data.Repository;
using Fanex.Logging;
using MassTransit;
using NSubstitute;
using Score247.Shared;
using Score247.Shared.Constants;
using Score247.Shared.Tests;
using Soccer.Core._Shared.Resources;
using Soccer.Core.Matches.Models;
using Soccer.Core.Notification.QueueMessages;
using Soccer.Core.Shared.Enumerations;
using Soccer.Database.Favorites.Criteria;
using Soccer.EventProcessors.Notifications;
using Soccer.EventProcessors.Shared.Configurations;
using Xunit;

namespace Soccer.EventProcessors.Tests.Notifications
{
    public class ReceiveMatchNotificationConsumerTests
    {
        private readonly IBus messageBus;
        private readonly IDynamicRepository dynamicRepository;
        private readonly ICacheManager cacheManager;
        private readonly ILogger logger;
        private readonly ILanguageResourcesService languageResources;
        private readonly IAppSettings appSettings;
        private readonly IMatchNotificationDeduper notificationDeduper;
        private readonly ConsumeContext<IMatchNotificationReceivedMessage> context;
        private readonly Fixture fixture;

        private readonly ReceiveMatchNotificationConsumer consumer;

        public ReceiveMatchNotificationConsumerTests() 
        {
            messageBus = Substitute.For<IBus>();
            dynamicRepository = Substitute.For<IDynamicRepository>();
            cacheManager = Substitute.For<ICacheManager>();
            logger = Substitute.For<ILogger>();
            languageResources = Substitute.For<ILanguageResourcesService>();
            appSettings = Substitute.For<IAppSettings>();
            notificationDeduper = Substitute.For<IMatchNotificationDeduper>();

            context = Substitute.For<ConsumeContext<IMatchNotificationReceivedMessage>>();
            fixture = new Fixture();

            appSettings.MaxUsersSent.Returns(2);

            consumer = new ReceiveMatchNotificationConsumer(
                messageBus, 
                dynamicRepository, 
                cacheManager, 
                logger, 
                languageResources, 
                appSettings, 
                notificationDeduper);
        }

        [Fact]
        public async Task Consume_Always_GetFavoriteUserIds()
        {
            // Arrange

            // Act
            await consumer.Consume(context);

            // Assert
            await dynamicRepository.Received(1).FetchAsync<string>(Arg.Any<GetUsersByFavoriteIdCriteria>());
        }

        [Fact]
        public async Task Consume_NotHaveAnyFavoriteUsers_MustNotGetAndCacheMatchInfo()
        {
            // Arrange
            var message = fixture.Create<MatchNotificationReceivedMessage>();
            context.Message.Returns(message);

            dynamicRepository
                .FetchAsync<string>(Arg.Any<GetUsersByFavoriteIdCriteria>())
                .Returns(new string[] { });

            // Act
            await consumer.Consume(context);

            // Assert
            await cacheManager.DidNotReceive().GetOrSetAsync(
                Arg.Is<string>(key => key == $"{CacheKeys.MATCH_INFO_CACHE_KEY}_{message.MatchId}"),
                Arg.Any<Func<Task<Match>>>(),
                Arg.Any<CacheItemOptions>());
        }

        [Fact]
        public async Task Consume_HasFavoriteUsers_GetAndCacheMatchInfo()
        {
            // Arrange
            var message = fixture.Create<MatchNotificationReceivedMessage>();
            context.Message.Returns(message);

            dynamicRepository
                .FetchAsync<string>(Arg.Any<GetUsersByFavoriteIdCriteria>())
                .Returns(new string[] { "user:1" });

            // Act
            await consumer.Consume(context);

            // Assert
            await cacheManager.Received(1).GetOrSetAsync(
                Arg.Is<string>(key => key == $"{CacheKeys.MATCH_INFO_CACHE_KEY}_{message.MatchId}"),
                Arg.Any<Func<Task<Match>>>(),
                Arg.Any<CacheItemOptions>());
        }

        [Fact]
        public async Task Consume_HasFavoriteUsers_MatchNotFound_NotPublishAnyProcessedNotification()
        {
            // Arrange
            var message = fixture.Create<MatchNotificationReceivedMessage>();
            context.Message.Returns(message);

            dynamicRepository
                .FetchAsync<string>(Arg.Any<GetUsersByFavoriteIdCriteria>())
                .Returns(new string[] { "user:1" });

            // Act
            await consumer.Consume(context);

            // Assert
            await messageBus.DidNotReceive().Publish(Arg.Any<IMatchNotificationProcessedMessage>());
        }

        [Fact]
        public async Task Consume_HasFavoriteUsers_NotSupportTimeline_NotPublishAnyProcessedNotification()
        {
            // Arrange
            var message = fixture.Create<MatchNotificationReceivedMessage>();
            context.Message.Returns(message);

            dynamicRepository
                .FetchAsync<string>(Arg.Any<GetUsersByFavoriteIdCriteria>())
                .Returns(new string[] { "user:1" });

            cacheManager
                .GetOrSetAsync(
                    Arg.Is<string>(key => key == $"{CacheKeys.MATCH_INFO_CACHE_KEY}_{message.MatchId}"),
                    Arg.Any<Func<Task<Match>>>(),
                    Arg.Any<CacheItemOptions>())
                .Returns(fixture.Create<Match>());

            // Act
            await consumer.Consume(context);

            // Assert
            await messageBus.DidNotReceive().Publish(Arg.Any<IMatchNotificationProcessedMessage>());
        }

        [Fact]
        public async Task Consume_HasFavoriteUsers_NotProcessed_PublishAnyProcessedNotification()
        {
            // Arrange
            var timeline = fixture.Create<TimelineEvent>()
                .With(timeline => timeline.Type, EventType.MatchStarted);

            var message = fixture.Create<MatchNotificationReceivedMessage>()
                .With(message => message.Timeline, timeline);
            
            context.Message.Returns(message);

            dynamicRepository
                .FetchAsync<string>(Arg.Any<GetUsersByFavoriteIdCriteria>())
                .Returns(new string[] { "user:1" });

            cacheManager
                .GetOrSetAsync(
                    Arg.Is<string>(key => key == $"{CacheKeys.MATCH_INFO_CACHE_KEY}_{message.MatchId}"),
                    Arg.Any<Func<Task<Match>>>(),
                    Arg.Any<CacheItemOptions>())
                .Returns(fixture.Create<Match>());

            // Act
            await consumer.Consume(context);

            // Assert
            await messageBus.Received(1).Publish(Arg.Any<IMatchNotificationProcessedMessage>());
        }

        [Fact]
        public async Task Consume_HasFavoriteUsers_Processed_NotPublishAnyProcessedNotification()
        {
            // Arrange
            var timeline = fixture.Create<TimelineEvent>()
                .With(timeline => timeline.Type, EventType.MatchStarted);

            var message = fixture.Create<MatchNotificationReceivedMessage>()
                .With(message => message.Timeline, timeline);

            context.Message.Returns(message);

            dynamicRepository
                .FetchAsync<string>(Arg.Any<GetUsersByFavoriteIdCriteria>())
                .Returns(new string[] { "user:1" });

            cacheManager
                .GetOrSetAsync(
                    Arg.Is<string>(key => key == $"{CacheKeys.MATCH_INFO_CACHE_KEY}_{message.MatchId}"),
                    Arg.Any<Func<Task<Match>>>(),
                    Arg.Any<CacheItemOptions>())
                .Returns(fixture.Create<Match>());

            notificationDeduper
                .IsProcessedAsync(
                    Arg.Is<string>(id => id == message.MatchId),
                    Arg.Any<string>())
                .Returns(true);

            // Act
            await consumer.Consume(context);

            // Assert
            await messageBus.DidNotReceive().Publish(Arg.Any<IMatchNotificationProcessedMessage>());
        }

        [Fact]
        public async Task Consume_HasFavoriteUsers_SendByBatch_PublishAnyProcessedNotification()
        {
            // Arrange
            var timeline = fixture.Create<TimelineEvent>()
                .With(timeline => timeline.Type, EventType.MatchStarted);

            var message = fixture.Create<MatchNotificationReceivedMessage>()
                .With(message => message.Timeline, timeline);

            context.Message.Returns(message);

            dynamicRepository
                .FetchAsync<string>(Arg.Any<GetUsersByFavoriteIdCriteria>())
                .Returns(new string[] { "user:1", "user:2", "user:3" });

            cacheManager
                .GetOrSetAsync(
                    Arg.Is<string>(key => key == $"{CacheKeys.MATCH_INFO_CACHE_KEY}_{message.MatchId}"),
                    Arg.Any<Func<Task<Match>>>(),
                    Arg.Any<CacheItemOptions>())
                .Returns(fixture.Create<Match>());

            // Act
            await consumer.Consume(context);

            // Assert
            await messageBus.Received(2).Publish(Arg.Any<IMatchNotificationProcessedMessage>());
        }
    }
}

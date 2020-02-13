using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using AutoFixture;
using Fanex.Caching;
using NSubstitute;
using Score247.Shared;
using Score247.Shared.Constants;
using Soccer.EventProcessors.Notifications;
using Xunit;

namespace Soccer.EventProcessors.Tests.Notifications
{
    public class MatchNotificationDeDuperTests
    {
        private readonly ICacheManager cacheManager;
        private readonly Fixture fixture;

        private readonly MatchNotificationDeduper deduper;

        public MatchNotificationDeDuperTests()
        {
            cacheManager = Substitute.For<ICacheManager>();
            fixture = new Fixture();

            deduper = new MatchNotificationDeduper(cacheManager);
        }

        [Fact]
        public async Task IsProcessedAsync_Always_GetOrSetCache()
        {
            var id = fixture.Create<string>();
            cacheManager
                .GetOrSetAsync(
                    Arg.Is<string>(key => key == $"{CacheKeys.MATCH_NOTIFICATION_CACHE_KEY}_{id}"),
                    Arg.Any<Func<Task<BlockingCollection<string>>>>(),
                    Arg.Any<CacheItemOptions>())
                .Returns(new BlockingCollection<string>());

            await deduper.IsProcessedAsync(id, fixture.Create<string>());

            await cacheManager.Received(1).GetOrSetAsync(
                Arg.Is<string>(key => key == $"{CacheKeys.MATCH_NOTIFICATION_CACHE_KEY}_{id}"),
                Arg.Any<Func<Task<BlockingCollection<string>>>>(),
                Arg.Any<CacheItemOptions>());
        }

        [Fact]
        public async Task IsProcessedAsync_CacheContains_ReturnTrue()
        {
            var id = fixture.Create<string>();
            var content = fixture.Create<string>();

            cacheManager
                .GetOrSetAsync(
                    Arg.Is<string>(key => key == $"{CacheKeys.MATCH_NOTIFICATION_CACHE_KEY}_{id}"),
                    Arg.Any<Func<Task<BlockingCollection<string>>>>(),
                    Arg.Any<CacheItemOptions>())
                .Returns(new BlockingCollection<string> { content });

            var processed = await deduper.IsProcessedAsync(id, content);

            Assert.True(processed);
        }

        [Fact]
        public async Task IsProcessedAsync_CacheNotContains_ReturnFalseAndAddedToCache()
        {
            var id = fixture.Create<string>();
            var content = fixture.Create<string>();
            var cacheItems = new BlockingCollection<string>();

            cacheManager
                .GetOrSetAsync(
                    Arg.Is<string>(key => key == $"{CacheKeys.MATCH_NOTIFICATION_CACHE_KEY}_{id}"),
                    Arg.Any<Func<Task<BlockingCollection<string>>>>(),
                    Arg.Any<CacheItemOptions>())
                .Returns(cacheItems);

            var processed = await deduper.IsProcessedAsync(id, content);

            Assert.False(processed);
            Assert.Contains(content, cacheItems);
        }
    }
}

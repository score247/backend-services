namespace Soccer.EventProcessors.Tests.Odds
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FakeItEasy;
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using MassTransit;
    using NSubstitute;
    using Score247.Shared.Tests;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Odds.Messages;
    using Soccer.Core.Odds.Models;
    using Soccer.Core.Shared.Enumerations;
    using Soccer.Database.Odds.Criteria;
    using Soccer.EventProcessors.Odds;
    using Soccer.EventProcessors.Shared.Configurations;
    using Xunit;

    [Trait("Soccer.EventProcessors", "Odds")]
    public class OddsChangeConsumerTests
    {
        private readonly IBus messageBus;
        private readonly IDynamicRepository dynamicRepository;
        private readonly Func<DateTime> getCurrentTimeFunc;
        private readonly ICacheService cacheService;
        private readonly OddsChangeConsumer oddsChangeConsumer;
        private readonly IAppSettings appSettings;
        private readonly ILogger logger;
        private readonly DateTime today = new DateTime(2019, 12, 12);

        public OddsChangeConsumerTests()
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            cacheService = Substitute.For<ICacheService>();
            messageBus = Substitute.For<IBus>();
            getCurrentTimeFunc = () => today;
            appSettings = Substitute.For<IAppSettings>();
            logger = Substitute.For<ILogger>();
            oddsChangeConsumer = new OddsChangeConsumer(
                dynamicRepository,
                getCurrentTimeFunc,
                messageBus,
                cacheService,
                appSettings,
                logger);
        }
#pragma warning disable S2699 // Tests should include assertions
        [Fact]
        public async Task Consume_NoMatchExist_DoesNotExecuteGetOdds()
        {
            cacheService.GetOrSetAsync(
                Arg.Any<string>(),
                Arg.Any<Func<IEnumerable<Match>>>(),
                Arg.Any<CacheItemOptions>()).Returns(Enumerable.Empty<Match>());

            await dynamicRepository.DidNotReceive().FetchAsync<BetTypeOdds>(Arg.Any<GetOddsCriteria>());
        }

        [Fact]
        public async Task Consume_OddsDoesNotBelongToAnyMatch_DoesNotExecuteFetchAsync()
        {
            StubMatches(today);
            var context = Substitute.For<ConsumeContext<IOddsChangeMessage>>();
            var oddsChangeMessage = new OddsChangeMessage(new List<MatchOdds>
            {
                new MatchOdds("11111", Enumerable.Empty<BetTypeOdds>())
            });
            context.Message.Returns(oddsChangeMessage);

            await oddsChangeConsumer.Consume(context);

            await dynamicRepository
                            .DidNotReceive()
                            .FetchAsync<BetTypeOdds>(Arg.Any<GetOddsCriteria>());
        }

        [Fact]
        public async Task Consume_TimelineEventIsNotSupportedOdds_DoesNotExecuteFetchAsync()
        {
            StubMatches(today);
            var context = Substitute.For<ConsumeContext<IOddsChangeMessage>>();
            var oddsChangeMessage = new OddsChangeMessage(new List<MatchOdds>
            {
                new MatchOdds("matchId", Enumerable.Empty<BetTypeOdds>())
            }, 
            new MatchEvent(
                "leagueId", 
                "matchId", 
                A.Dummy<MatchResult>(),
                A.Dummy<TimelineEvent>()));
            context.Message.Returns(oddsChangeMessage);

            await oddsChangeConsumer.Consume(context);

            await dynamicRepository
                            .DidNotReceive()
                            .FetchAsync<BetTypeOdds>(Arg.Any<GetOddsCriteria>());
        }


        private static MatchOdds StubMatchOdds(DateTime? lastUpdatedTime = null)
        {
            var lastUpdate = lastUpdatedTime ?? new DateTimeOffset(2019, 1, 2, 0, 0, 0, new TimeSpan(7, 0, 0)).DateTime;

            return new MatchOdds(
                "matchId1",
                new List<BetTypeOdds>
                {
                    StubOneXTwoBetTypeOdds("sr:book:201", "bookmakername 1", lastUpdate, 0.2m, 0.2m),
                    StubOneXTwoBetTypeOdds("sr:book:202", "bookmakername 2", lastUpdate, 0.3m, 0.3m)
                },
                lastUpdate);
        }

        private static BetTypeOdds StubOneXTwoBetTypeOdds(
            string bookMakerId = "sr:book:201",
            string bookMarkerName = "bookmakername 1",
            DateTime? lastUpdated = null,
            decimal oddsOffset = 0m,
            decimal openingOddsOffset = 0m)
            => new BetTypeOdds(
                1,
                "1x2",
                new Bookmaker(bookMakerId, bookMarkerName),
                lastUpdated ?? new DateTime(2019, 1, 2),
                new List<BetOptionOdds>
                {
                    new BetOptionOdds("Home", 1.2m + oddsOffset, 1.5m + openingOddsOffset, "1", "2"),
                    new BetOptionOdds("Away", 1.3m + oddsOffset, 1.4m + openingOddsOffset, "1", "2"),
                    new BetOptionOdds("Draw", 1.1m + oddsOffset, 1.6m + openingOddsOffset, "1", "2")
                });

        private static List<BetTypeOdds> StubBetTypeOddsListForOddsMovement(
            bool returnOddsAfterMatchStarted = false,
            DateTime? eventDate = null,
            string bookMakerId = "sr:book:201",
            string bookMakerName = "bookmakername 1")
        {
            var matchStartTime = eventDate.HasValue ? eventDate.Value : new DateTime(2019, 1, 1);

            return new List<BetTypeOdds>
            {
                StubOneXTwoBetTypeOdds(bookMakerId, bookMakerName, matchStartTime.AddMinutes(-120), 0.11m),
                StubOneXTwoBetTypeOdds(bookMakerId, bookMakerName, matchStartTime.AddMinutes(-80), 0.12m),
                StubOneXTwoBetTypeOdds(bookMakerId, bookMakerName, matchStartTime.AddMinutes(-40), 0.13m),
                StubOneXTwoBetTypeOdds(bookMakerId, bookMakerName, matchStartTime.AddMinutes(-20), 0.14m),
                StubOneXTwoBetTypeOdds(bookMakerId, bookMakerName, matchStartTime.AddMinutes(-10), 0.15m),
                StubOneXTwoBetTypeOdds(bookMakerId, bookMakerName, matchStartTime, 0.15m),
                StubOneXTwoBetTypeOdds(bookMakerId, bookMakerName, matchStartTime.AddMinutes(10), 0.16m),
                StubOneXTwoBetTypeOdds(bookMakerId, bookMakerName, matchStartTime.AddMinutes(20), 0.17m),
                StubOneXTwoBetTypeOdds(bookMakerId, bookMakerName, matchStartTime.AddMinutes(50), 0.18m),
                StubOneXTwoBetTypeOdds(bookMakerId, bookMakerName, matchStartTime.AddMinutes(55), 0.19m),
                StubOneXTwoBetTypeOdds(bookMakerId, bookMakerName, matchStartTime.AddMinutes(62), 0.20m),
                StubOneXTwoBetTypeOdds(bookMakerId, bookMakerName, matchStartTime.AddMinutes(75), 0.21m),
                StubOneXTwoBetTypeOdds(bookMakerId, bookMakerName, matchStartTime.AddMinutes(85), 0.22m),
                StubOneXTwoBetTypeOdds(bookMakerId, bookMakerName, matchStartTime.AddMinutes(88), 0.23m),
                StubOneXTwoBetTypeOdds(bookMakerId, bookMakerName, matchStartTime.AddMinutes(92), 0.24m),
                StubOneXTwoBetTypeOdds(bookMakerId, bookMakerName, matchStartTime.AddMinutes(98), 0.25m)
            }
            .Where(betTypeOdds => returnOddsAfterMatchStarted || betTypeOdds.LastUpdatedTime < matchStartTime)
            .ToList();
        }


        private void StubMatches(DateTime fromDate)
        {
            var matches = new List<Match>
            {
                A.Dummy<Match>().With(match => match.Id, "matchId")
            };

            cacheService.GetOrSetAsync(
                $"Odds_Match_{fromDate.ToUniversalTime().ToString("ddMMyyyy")}",
                Arg.Any<Func<IEnumerable<Match>>>(),
                Arg.Any<CacheItemOptions>())
                .Returns(matches);
        }

#pragma warning restore S2699 // Tests should include assertions
    }
}
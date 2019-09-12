namespace Soccer.API.Tests.Odds
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using NSubstitute;
    using Soccer.API.Odds;
    using Soccer.API.Shared.Configurations;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Odds.Models;
    using Soccer.Core.Shared.Enumerations;
    using Soccer.Database.Matches.Criteria;
    using Soccer.Database.Odds.Criteria;
    using Xunit;

    [Trait("Soccer.API", "Odds")]
    public class OddsQueryServiceTests
    {
        private readonly OddsQueryService oddsServiceImpl;
        private readonly IDynamicRepository dynamicRepository;
        private readonly IAppSettings appSettings;
        private readonly ICacheService cacheService;

        private const string globalMatchId = "matchId1";
        private const int globalBetTypeId = 1;
        private readonly DateTime eventDate = new DateTime(2019, 2, 3);

        public OddsQueryServiceTests()
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            appSettings = Substitute.For<IAppSettings>();
            cacheService = Substitute.For<ICacheService>();
            appSettings.NumOfDaysToShowOddsBeforeKickoffDate.Returns(356);
            oddsServiceImpl = new OddsQueryService(dynamicRepository, appSettings, cacheService);
        }

        [Fact]
        public async Task GetOdds_Always_ReturnMatchId()
        {
            StubBetTypeOdds();

            var matchOdds = await oddsServiceImpl.GetOdds(globalMatchId, globalBetTypeId, Language.en_US);

            Assert.Equal(globalMatchId, matchOdds.MatchId);
        }

        private IEnumerable<BetTypeOdds> StubBetTypeOdds(
            string stubMatchId = null,
            IEnumerable<BetTypeOdds> betTypeOddsList = null,
            string bookmakerId = null)
        {
            betTypeOddsList = betTypeOddsList ?? StubMatchOdds().BetTypeOddsList;
            stubMatchId = stubMatchId ?? globalMatchId;

            dynamicRepository
                .FetchAsync<BetTypeOdds>(
                    Arg.Is<GetOddsCriteria>(
                        criteria => criteria.BetTypeId == globalBetTypeId
                            && criteria.MatchId == stubMatchId))
                .Returns(betTypeOddsList.Where(bto => bookmakerId == null || bto.Bookmaker.Id == bookmakerId));

            return betTypeOddsList;
        }

        [Fact]
        public async Task GetOdds_Always_ReturnBetTypeOddsList()
        {
            StubBetTypeOdds();

            var actualBetTypeOddsList = await oddsServiceImpl.GetOdds(globalMatchId, globalBetTypeId, Language.en_US);

            var firstBetTypeOdds = actualBetTypeOddsList.BetTypeOddsList.First();
            var secondBetTypeOdds = actualBetTypeOddsList.BetTypeOddsList.ElementAt(1);

            Assert.Equal(2, actualBetTypeOddsList.BetTypeOddsList.Count());
            //Order by Bookmaker Name
            Assert.Equal("sr:book:201", firstBetTypeOdds.Bookmaker.Id);
            Assert.Equal("sr:book:202", secondBetTypeOdds.Bookmaker.Id);
            //Get last odds for each bookmaker
            Assert.Equal(1.4m, firstBetTypeOdds.BetOptions.ElementAt(0).LiveOdds);
            Assert.Equal(1.5m, secondBetTypeOdds.BetOptions.ElementAt(0).LiveOdds);
        }

        ////[Fact]
        ////public async Task GetBetTypeOddsList_IsNotNewOdds_DontCallInsertRange()
        ////{
        ////    dynamicRepository.Query(Arg.Any<Func<OddsEntity, bool>>()).Returns(StubBetTypeOddsList());
        ////    var matchOdds = StubMatchOdds();

        ////    await oddsServiceImpl.InsertOdds(matchOdds);

        ////    await dynamicRepository.DidNotReceive().InsertRange(Arg.Any<IEnumerable<OddsEntity>>());
        ////}

        ////[Fact]
        ////public async Task GetBetTypeOddsList_IsNewOdds_CallInsertRange()
        ////{
        ////    var lastUpdatedTime = new DateTimeOffset(2019, 4, 2, 0, 0, 0, new TimeSpan(7, 0, 0)).DateTime;
        ////    dynamicRepository.Query(Arg.Any<Func<OddsEntity, bool>>()).Returns(StubBetTypeOddsList());
        ////    var matchOdds = StubMatchOdds(lastUpdatedTime);

        ////    await oddsServiceImpl.InsertOdds(matchOdds);

        ////    await dynamicRepository.Received().InsertRange(
        ////        Arg.Is<IEnumerable<OddsEntity>>(
        ////            entities => entities.Count() == 2
        ////                        && entities.First().BookmakerId == "sr:book:201"
        ////                        && entities.First().MatchId == "matchId1"
        ////                        && entities.ElementAt(1).BookmakerId == "sr:book:202"
        ////        ));
        ////}

        [Fact]
        public async Task GetBetTypeOddsList_Always_AssignOpeningOdds()
        {
            var betTypeOddsList = new List<BetTypeOdds>
                {
                    StubOneXTwoBetTypeOdds("sr:book:201", "bookmakername 1", new DateTime(2019, 2, 1), 0.2m, 0.2m),
                    StubOneXTwoBetTypeOdds("sr:book:201", "bookmakername 1", new DateTime(2019, 3, 1), 0.3m, 0.3m)
                };
            StubBetTypeOdds(betTypeOddsList: betTypeOddsList);

            var actualBetTypeOddsList = await oddsServiceImpl.GetOdds(globalMatchId, globalBetTypeId, Language.en_US);

            var firstBetTypeOdds = actualBetTypeOddsList.BetTypeOddsList.First();

            Assert.Equal(1.7m, firstBetTypeOdds.BetOptions.First().OpeningOdds);
            Assert.Equal(1.5m, firstBetTypeOdds.BetOptions.First().LiveOdds);
            Assert.Equal(1.6m, firstBetTypeOdds.BetOptions.ElementAt(1).OpeningOdds);
            Assert.Equal(1.8m, firstBetTypeOdds.BetOptions.ElementAt(2).OpeningOdds);
        }

        [Fact]
        public async Task GetOddsMovement_NotFoundBookmaker_ReturnEmpty()
        {
            var matchOddsMovement = await oddsServiceImpl.GetOddsMovement("matchIdNoBookmaker", 1, "bookmarker", Language.en_US);

            Assert.Null(matchOddsMovement.Bookmaker);
            Assert.Null(matchOddsMovement.OddsMovements);
        }

        [Fact]
        public async Task GetOddsMovement_BookMakerHasData_BookMakerInfo()
        {
            var oddsMovementMatchId = "oddsMovementMatchId";
            var bookmakerId = "sr:book:201";
            var betTypeOddsList = new List<BetTypeOdds>
                {
                    StubOneXTwoBetTypeOdds(bookmakerId, "bookmakername 1", new DateTime(2019, 2, 1), 0.2m, 0.2m),
                    StubOneXTwoBetTypeOdds(bookmakerId, "bookmakername 1", new DateTime(2019, 3, 1), 0.3m, 0.3m)
                };
            StubBetTypeOdds(oddsMovementMatchId, betTypeOddsList);

            var matchOddsMovement = await oddsServiceImpl.GetOddsMovement(oddsMovementMatchId, 1, bookmakerId, Language.en_US);

            Assert.Equal(oddsMovementMatchId, matchOddsMovement.MatchId);
            Assert.Equal(bookmakerId, matchOddsMovement.Bookmaker.Id);
        }

        [Fact]
        public async Task GetOddsMovement_MatchDoesNotExist_ReturnEmptyOddsMovement()
        {
            var oddsMovementMatchId = "GetOddsMovement_MatchDoesNotExist_ReturnEmptyOddsMovement";
            StubBetTypeOdds(oddsMovementMatchId);
            dynamicRepository
                .GetAsync<Match>(Arg.Is<GetMatchByIdCriteria>(c => c.Id == oddsMovementMatchId && c.Language == Language.en_US.DisplayName))
                .Returns(Task.FromResult<Match>(null));

            var matchOddsMovement = await oddsServiceImpl.GetOddsMovement(oddsMovementMatchId, 1, "bookMakerId", Language.en_US);

            Assert.Empty(matchOddsMovement.OddsMovements);
        }

        [Fact]
        public async Task GetOddsMovement_MatchDidNotStart_ReturnOddsMovementBeforeMatchStarted()
        {
            // Arrange
            var matchId = "GetOddsMovement_MatchDidNotStart_ReturnOddsMovementBeforeMatchStarted";
            var bookmakerId = "bookMakerId";
            var betTypeOddsList = StubBetTypeOddsListForOddsMovement();
            StubBetTypeOdds(matchId, betTypeOddsList);

            StubMatch(matchId);

            // Act
            var matchOddsMovement = await oddsServiceImpl.GetOddsMovement(matchId, 1, bookmakerId, Language.en_US);

            // Assert
            Assert.Equal(5, matchOddsMovement.OddsMovements.Count());

            var openingOdds = matchOddsMovement.OddsMovements.Last();
            Assert.Equal("Opening", openingOdds.MatchTime);
            Assert.False(openingOdds.IsMatchStarted);
            Assert.Equal(1.5m, openingOdds.BetOptions.First().LiveOdds);
            Assert.Equal(1.5m, openingOdds.BetOptions.First().OpeningOdds);

            var firstOdds = matchOddsMovement.OddsMovements.First();
            Assert.Equal("Live", firstOdds.MatchTime);
            Assert.False(firstOdds.IsMatchStarted);
            Assert.Equal(1.35m, firstOdds.BetOptions.First().LiveOdds);

            Assert.Equal("Live", matchOddsMovement.OddsMovements.ElementAt(1).MatchTime);
        }

        [Fact]
        public async Task GetOddsMovement_MatchJustKickedOff_ReturnOddsWithKickoffInfo()
        {
            // Arrange
            var matchId = "GetOddsMovement_MatchJustKickedOff_ReturnOddsWithKickoffInfo";
            var bookMakerId = "bookMakerId";
            var timeLines = new List<TimelineEvent>
            {
                new TimelineEvent { Type = EventType.PeriodStart, Period = 1, Time = eventDate, PeriodType = PeriodType.RegularPeriod }
            };
            var match = StubMatch(matchId, eventDate, timeLines);

            var betTypeOddsList = new List<BetTypeOdds>
            {
                StubOneXTwoBetTypeOdds(bookMakerId, "bookMakerName", match.EventDate.AddMinutes(-10).DateTime, 0.15m, 0.1m),
                StubOneXTwoBetTypeOdds(bookMakerId, "bookMakerName", match.EventDate.AddMinutes(-5).DateTime, 0.15m, 0.1m),
                StubOneXTwoBetTypeOdds(bookMakerId, "bookMakerName", match.EventDate.DateTime, 0.16m)
            };
            StubBetTypeOdds(matchId, betTypeOddsList);

            // Act
            var matchOddsMovement = await oddsServiceImpl.GetOddsMovement(matchId, 1, bookMakerId, Language.en_US);

            // Assert
            Assert.Equal(3, matchOddsMovement.OddsMovements.Count());

            var kickoffOdds = matchOddsMovement.OddsMovements.First();
            Assert.Equal("KO", kickoffOdds.MatchTime);
            Assert.True(kickoffOdds.IsMatchStarted);
            Assert.Equal(0, kickoffOdds.HomeScore);
            Assert.Equal(0, kickoffOdds.AwayScore);
            Assert.Equal(1.36m, kickoffOdds.BetOptions.First().LiveOdds);
            Assert.Equal(OddsTrend.Up, kickoffOdds.BetOptions.First().OddsTrend);
        }

        [Fact]
        public async Task GetOddsMovement_ScoreChangeInFirstHalf_ReturnScoreChangeOdds()
        {
            // Arrange
            var matchId = "GetOddsMovement_ScoreChangeInFirstHalf_ReturnScoreChangeOdds";
            var bookMakerId = "bookMakerId";
            var timeLines = new List<TimelineEvent>
            {
                new TimelineEvent { Type = EventType.PeriodStart, Period = 1, Time = eventDate, PeriodType = PeriodType.RegularPeriod },
                new TimelineEvent { Type = EventType.ScoreChange, PeriodType = PeriodType.RegularPeriod, HomeScore = 1, AwayScore = 1, MatchTime = 20, Time = eventDate.AddMinutes(20) }
            };
            var match = StubMatch(matchId, eventDate, timeLines);

            var betTypeOddsList = new List<BetTypeOdds>
            {
                StubOneXTwoBetTypeOdds(bookMakerId, "bookMakerName", match.EventDate.AddMinutes(-10).DateTime, 0.15m, 0.1m),
                StubOneXTwoBetTypeOdds(bookMakerId, "bookMakerName", match.EventDate.DateTime, 0.16m),
                StubOneXTwoBetTypeOdds(bookMakerId, "bookMakerName", match.EventDate.AddMinutes(20).DateTime, 0.14m)
            };
            StubBetTypeOdds(matchId, betTypeOddsList, bookMakerId);

            // Act
            var matchOddsMovement = await oddsServiceImpl.GetOddsMovement(matchId, 1, bookMakerId, Language.en_US);

            // Assert
            Assert.Equal(3, matchOddsMovement.OddsMovements.Count());

            var firstHalfScoreOdds = matchOddsMovement.OddsMovements.First();
            Assert.Equal("20'", firstHalfScoreOdds.MatchTime);
            Assert.True(firstHalfScoreOdds.IsMatchStarted);
            Assert.Equal(1, firstHalfScoreOdds.HomeScore);
            Assert.Equal(1, firstHalfScoreOdds.AwayScore);
            Assert.Equal(1.34m, firstHalfScoreOdds.BetOptions.First().LiveOdds);
            Assert.Equal(OddsTrend.Down, firstHalfScoreOdds.BetOptions.First().OddsTrend);
        }

        [Fact]
        public async Task GetOddsMovement_OddChangeInHalfTimeBreak_ReturnHalfTimeOdds()
        {
            // Arrange
            var matchId = "GetOddsMovement_OddChangeInHalfTimeBreak_ReturnHalfTimeOdds";
            var bookMakerId = "bookMakerId";
            var timeLines = new List<TimelineEvent>
            {
                new TimelineEvent { Type = EventType.PeriodStart, Period = 1, Time = eventDate, PeriodType = PeriodType.RegularPeriod },
                new TimelineEvent { Type = EventType.BreakStart, PeriodType = PeriodType.Pause, Time = eventDate.AddMinutes(55) }
            };
            var match = StubMatch(matchId, eventDate, timeLines);

            var betTypeOddsList = new List<BetTypeOdds>
            {
                StubOneXTwoBetTypeOdds(bookMakerId, "bookMakerName", match.EventDate.DateTime, 0.16m),
                StubOneXTwoBetTypeOdds(bookMakerId, "bookMakerName", match.EventDate.AddMinutes(55).DateTime, 0.474m)
            };
            StubBetTypeOdds(matchId, betTypeOddsList, bookMakerId);

            // Act
            var matchOddsMovement = await oddsServiceImpl.GetOddsMovement(matchId, 1, bookMakerId, Language.en_US);

            // Assert
            Assert.Equal(3, matchOddsMovement.OddsMovements.Count());

            var halfTimeOdds = matchOddsMovement.OddsMovements.First();
            Assert.Equal("HT", halfTimeOdds.MatchTime);
            Assert.True(halfTimeOdds.IsMatchStarted);
            Assert.Equal(0, halfTimeOdds.HomeScore);
            Assert.Equal(0, halfTimeOdds.AwayScore);
            Assert.Equal(1.674m, halfTimeOdds.BetOptions.First().LiveOdds);
            Assert.Equal(OddsTrend.Up, halfTimeOdds.BetOptions.First().OddsTrend);
        }

        [Fact]
        public async Task GetOddsMovement_SecondHalfStart_ReturnSecondHalfStartOdds()
        {
            // Arrange
            var matchId = "GetOddsMovement_SecondHalfStart_ReturnSecondHalfStartOdds";
            var bookMakerId = "bookMakerId";
            var timeLines = new List<TimelineEvent>
            {
                new TimelineEvent { Type = EventType.PeriodStart, Period = 1, Time = eventDate, PeriodType = PeriodType.RegularPeriod },
                new TimelineEvent { Type = EventType.ScoreChange, PeriodType = PeriodType.RegularPeriod, HomeScore = 1, AwayScore = 1, MatchTime = 20, Time = eventDate.AddMinutes(20) },
                new TimelineEvent { Type = EventType.BreakStart, PeriodType = PeriodType.Pause, Time = eventDate.AddMinutes(55) },
                new TimelineEvent { Type = EventType.PeriodStart, Period = 2, Time = eventDate.AddMinutes(70), PeriodType = PeriodType.RegularPeriod }
            };
            var match = StubMatch(matchId, eventDate, timeLines);

            var betTypeOddsList = new List<BetTypeOdds>
            {
                StubOneXTwoBetTypeOdds(bookMakerId, "bookMakerName", match.EventDate.DateTime, 0.16m),
                StubOneXTwoBetTypeOdds(bookMakerId, "bookMakerName", match.EventDate.AddMinutes(20).DateTime, 0.14m),
                StubOneXTwoBetTypeOdds(bookMakerId, "bookMakerName", match.EventDate.AddMinutes(55).DateTime, 0.474m),
                StubOneXTwoBetTypeOdds(bookMakerId, "bookMakerName", match.EventDate.AddMinutes(70).DateTime, 0.574m)
            };
            StubBetTypeOdds(matchId, betTypeOddsList, bookMakerId);

            // Act
            var matchOddsMovement = await oddsServiceImpl.GetOddsMovement(matchId, 1, bookMakerId, Language.en_US);

            // Assert
            Assert.Equal(5, matchOddsMovement.OddsMovements.Count());

            var secondHalfStartOdds = matchOddsMovement.OddsMovements.First();
            Assert.Equal("46'", secondHalfStartOdds.MatchTime);
            Assert.True(secondHalfStartOdds.IsMatchStarted);
            Assert.Equal(1, secondHalfStartOdds.HomeScore);
            Assert.Equal(1, secondHalfStartOdds.AwayScore);
            Assert.Equal(1.774m, secondHalfStartOdds.BetOptions.First().LiveOdds);
            Assert.Equal(OddsTrend.Up, secondHalfStartOdds.BetOptions.First().OddsTrend);
        }

        [Fact]
        public async Task GetOddsMovement_ScoreChangeInSecondHalf_ReturnScoreChangeOdds()
        {
            // Arrange
            var matchId = "GetOddsMovement_ScoreChangeInSecondHalf_ReturnScoreChangeOdds";
            var bookMakerId = "bookMakerId";
            var timeLines = new List<TimelineEvent>
            {
                new TimelineEvent { Type = EventType.PeriodStart, Period = 1, Time = eventDate, PeriodType = PeriodType.RegularPeriod },
                new TimelineEvent { Type = EventType.BreakStart, PeriodType = PeriodType.Pause, Time = eventDate.AddMinutes(55) },
                new TimelineEvent { Type = EventType.PeriodStart, Period = 2, Time = eventDate.AddMinutes(70), PeriodType = PeriodType.RegularPeriod },
                new TimelineEvent { Type = EventType.ScoreChange, PeriodType = PeriodType.RegularPeriod, HomeScore = 2, AwayScore = 1, MatchTime = 56, Time = eventDate.AddMinutes(80) }
            };
            var match = StubMatch(matchId, eventDate, timeLines);

            var betTypeOddsList = new List<BetTypeOdds>
            {
                StubOneXTwoBetTypeOdds(bookMakerId, "bookMakerName", match.EventDate.DateTime, 0.16m),
                StubOneXTwoBetTypeOdds(bookMakerId, "bookMakerName", match.EventDate.AddMinutes(55).DateTime, 0.474m),
                StubOneXTwoBetTypeOdds(bookMakerId, "bookMakerName", match.EventDate.AddMinutes(70).DateTime, 0.574m),
                StubOneXTwoBetTypeOdds(bookMakerId, "bookMakerName", match.EventDate.AddMinutes(80).DateTime, 0.14m),
            };
            StubBetTypeOdds(matchId, betTypeOddsList, bookMakerId);

            // Act
            var matchOddsMovement = await oddsServiceImpl.GetOddsMovement(matchId, 1, bookMakerId, Language.en_US);

            // Assert
            Assert.Equal(5, matchOddsMovement.OddsMovements.Count());

            var secondHalfStartOdds = matchOddsMovement.OddsMovements.First();
            Assert.Equal("56'", secondHalfStartOdds.MatchTime);
            Assert.True(secondHalfStartOdds.IsMatchStarted);
            Assert.Equal(2, secondHalfStartOdds.HomeScore);
            Assert.Equal(1, secondHalfStartOdds.AwayScore);
            Assert.Equal(1.34m, secondHalfStartOdds.BetOptions.First().LiveOdds);
            Assert.Equal(OddsTrend.Down, secondHalfStartOdds.BetOptions.First().OddsTrend);
        }

        [Fact]
        public async Task GetOddsMovement_OddsChangeInSecondHalf_ReturnOddsChange()
        {
            // Arrange
            var matchId = "GetOddsMovement_OddsChangeInSecondHalf_ReturnOddsChange";
            var bookMakerId = "bookMakerId";
            var timeLines = new List<TimelineEvent>
            {
                new TimelineEvent { Type = EventType.PeriodStart, Period = 1, Time = eventDate, PeriodType = PeriodType.RegularPeriod },
                new TimelineEvent { Type = EventType.BreakStart, PeriodType = PeriodType.Pause, Time = eventDate.AddMinutes(55) },
                new TimelineEvent { Type = EventType.PeriodStart, Period = 2, Time = eventDate.AddMinutes(70), PeriodType = PeriodType.RegularPeriod },
                new TimelineEvent { Type = EventType.ScoreChange, PeriodType = PeriodType.RegularPeriod, HomeScore = 2, AwayScore = 1, MatchTime = 56, Time = eventDate.AddMinutes(80) }
            };
            var match = StubMatch(matchId, eventDate, timeLines);

            var betTypeOddsList = new List<BetTypeOdds>
            {
                StubOneXTwoBetTypeOdds(bookMakerId, "bookMakerName", match.EventDate.DateTime, 0.16m),
                StubOneXTwoBetTypeOdds(bookMakerId, "bookMakerName", match.EventDate.AddMinutes(55).DateTime, 0.474m),
                StubOneXTwoBetTypeOdds(bookMakerId, "bookMakerName", match.EventDate.AddMinutes(70).DateTime, 0.574m),
                StubOneXTwoBetTypeOdds(bookMakerId, "bookMakerName", match.EventDate.AddMinutes(80).DateTime, 0.14m),
                StubOneXTwoBetTypeOdds(bookMakerId, "bookMakerName", match.EventDate.AddMinutes(90).DateTime, 0.12m),
            };
            StubBetTypeOdds(matchId, betTypeOddsList, bookMakerId);

            // Act
            var matchOddsMovement = await oddsServiceImpl.GetOddsMovement(matchId, 1, bookMakerId, Language.en_US);

            // Assert
            Assert.Equal(6, matchOddsMovement.OddsMovements.Count());

            var secondHalfStartOdds = matchOddsMovement.OddsMovements.First();
            Assert.Equal("66'", secondHalfStartOdds.MatchTime);
            Assert.True(secondHalfStartOdds.IsMatchStarted);
            Assert.Equal(2, secondHalfStartOdds.HomeScore);
            Assert.Equal(1, secondHalfStartOdds.AwayScore);
            Assert.Equal(1.32m, secondHalfStartOdds.BetOptions.First().LiveOdds);
            Assert.Equal(OddsTrend.Down, secondHalfStartOdds.BetOptions.First().OddsTrend);
        }

        public Match StubMatch(
            string matchId = "matchId1",
            DateTime? eventDate = null,
            IEnumerable<TimelineEvent> timelines = null)
        {
            var match = new Match
            {
                Id = matchId,
                EventDate = eventDate.HasValue
                    ? eventDate.Value
                    : new DateTime(2019, 2, 3),
            };

            dynamicRepository
                .GetAsync<Match>(Arg.Is<GetMatchByIdCriteria>(c => c.Id == matchId && c.Language == Language.en_US.DisplayName))
                .Returns(Task.FromResult<Match>(match));

            dynamicRepository
                .FetchAsync<TimelineEvent>(Arg.Is<GetTimelineEventsCriteria>(c => c.MatchId == matchId))
                .Returns(timelines);

            match.TimeLines = timelines ?? Enumerable.Empty<TimelineEvent>();

            return match;
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
    }
}
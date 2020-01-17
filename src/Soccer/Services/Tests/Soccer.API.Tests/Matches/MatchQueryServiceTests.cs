using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FakeItEasy;
using Fanex.Caching;
using Fanex.Data.Repository;
using NSubstitute;
using Score247.Shared;
using Score247.Shared.Tests;
using Soccer.API.Matches;
using Soccer.API.Matches.Models;
using Soccer.API.Shared.Configurations;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.Core.Teams.Models;
using Soccer.Database.Matches.Criteria;
using Xunit;

namespace Soccer.API.Tests.Matches
{
    [Trait("Soccer.API", "Match")]
    public class MatchQueryServiceTests
    {
        private const string FormatDate = "yyyyMMdd-hhmmss";

        private readonly MatchQueryService matchQueryService;
        private readonly IDynamicRepository dynamicRepository;
        private readonly ICacheManager cacheManager;
        private readonly Func<DateTimeOffset> currentTimeFunc;
        private readonly Fixture fixture;

        public MatchQueryServiceTests()
        {
            fixture = new Fixture();

            dynamicRepository = Substitute.For<IDynamicRepository>();
            cacheManager = Substitute.For<ICacheManager>();
            currentTimeFunc = Substitute.For<Func<DateTimeOffset>>();
            currentTimeFunc().Returns(DateTimeOffset.Now);

            var appSettings = Substitute.For<IAppSettings>();
            appSettings.MatchShortCacheTimeDuration.Returns(1);
            appSettings.MatchLongCacheTimeDuration.Returns(2);

            matchQueryService = new MatchQueryService(dynamicRepository, cacheManager, appSettings, currentTimeFunc);
        }

        [Fact]
#pragma warning disable S2699 // Tests should include assertions
        public async Task GetMatchCommentary_ExecuteGetOrSetAsync()
#pragma warning restore S2699 // Tests should include assertions
        {
            await matchQueryService.GetMatchCommentary("sr:match:1", Language.en_US, default(DateTimeOffset));

            await cacheManager.Received(1).GetOrSetAsync(
                "MatchQuery_MatchCommentaryCacheKey_sr:match:1",
                Arg.Any<Func<Task<IEnumerable<TimelineEvent>>>>(),
                Arg.Any<CacheItemOptions>());
        }

        [Fact]
        public async Task GetByDateRange_Current_GetOrSetAsyncFromCache()
        {
            var from = DateTimeOffset.Now;
            var to = DateTimeOffset.Now.AddDays(1);

            await matchQueryService.GetByDateRange(from, to, Language.en_US);

            await cacheManager.Received(1).GetOrSetAsync(
                $"MatchQuery_MatchListCacheKey_{from.ToString(FormatDate)}_{to.ToString(FormatDate)}",
                Arg.Any<Func<Task<IEnumerable<Match>>>>(),
                Arg.Any<CacheItemOptions>());
        }

        [Fact]
        public async Task GetByDateRange_Former_FetchAsyncFromRepository()
        {
            var from = DateTimeOffset.Now.AddDays(-4);
            var to = DateTimeOffset.Now.AddDays(-3);

            await matchQueryService.GetByDateRange(from, to, Language.en_US);

            await dynamicRepository.Received(1).FetchAsync<Match>(Arg.Any<GetMatchesByDateRangeCriteria>());
        }

        [Fact]
        public async Task GetByDateRange_Always_SortByLeagueOrde()
        {
            var from = DateTimeOffset.Now.AddDays(-4);
            var to = DateTimeOffset.Now.AddDays(-2);

            dynamicRepository.FetchAsync<Match>(Arg.Any<GetMatchesByDateRangeCriteria>()).Returns(new List<Match>
            {
                fixture.Create<Match>().With(match => match.League, fixture.Create<League>().With(league => league.Order, 1)),
                fixture.Create<Match>().With(match => match.League, fixture.Create<League>().With(league => league.Order, 3))
            });

            cacheManager
                .GetOrSetAsync(
                    $"MatchQuery_MatchListCacheKey_{from.ToString(FormatDate)}_{to.ToString(FormatDate)}",
                    Arg.Any<Func<Task<IEnumerable<Match>>>>(),
                    Arg.Any<CacheItemOptions>())
                .Returns(new List<Match> {
                    fixture.Create<Match>().With(match => match.League, fixture.Create<League>().With(league => league.Order, 2))
                });

            var matches = await matchQueryService.GetByDateRange(from, to, Language.en_US);

            Assert.Equal(1, matches.First().LeagueOrder);
            Assert.Equal(3, matches.Last().LeagueOrder);
        }

        #region Match Statistic

        [Fact]
#pragma warning disable S2699 // Tests should include assertions
        public async Task GetMatchStatisticData_Always_GetFromCache()
#pragma warning restore S2699 // Tests should include assertions
        {
            await matchQueryService.GetMatchStatistic("sr:match:1", default(DateTimeOffset));

            await cacheManager.Received(1).GetOrSetAsync(
                "MatchQuery_MatchStatisticCacheKey_sr:match:1",
                Arg.Any<Func<Task<MatchStatistic>>>(),
                Arg.Any<CacheItemOptions>());
        }

        [Fact]
#pragma warning disable S2699 // Tests should include assertions
        public async Task GetMatchStatisticData_Always_ExecuteGetAsync()
#pragma warning restore S2699 // Tests should include assertions
        {
            var matchId = "matchId";
            await matchQueryService.GetMatchStatisticData(matchId, default(DateTimeOffset));

            await dynamicRepository.Received(1).GetAsync<Match>(
                Arg.Is<GetMatchByIdCriteria>(criteria => criteria.Id == matchId));
        }

        [Fact]
        public async Task GetMatchStatisticData_NoData_ReturnEmptyMatchStatistic()
        {
            var matchId = "matchId";
            StubMatchStatistic(matchId, null);

            var match = await matchQueryService.GetMatchStatisticData(matchId, default(DateTimeOffset));

            Assert.Null(match.MatchId);
        }

        [Fact]
        public async Task GetMatchStatisticData_HasData_ReturnMatchStatistic()
        {
            var matchId = "matchId";
            var returnedMatch = A.Dummy<Match>()
                        .With(t => t.Teams, new List<Team>
                        {
                            A.Dummy<Team>().With(t => t.IsHome, true).With(t => t.Statistic, new TeamStatistic(1, 1, 1)),
                            A.Dummy<Team>().With(t => t.IsHome, false).With(t => t.Statistic, new TeamStatistic(2, 2, 1))
                        });
            StubMatchStatistic(matchId, returnedMatch);

            var match = await matchQueryService.GetMatchStatisticData(matchId, default(DateTimeOffset));

            Assert.Equal(1, match.HomeStatistic.RedCards);
            Assert.Equal(2, match.AwayStatistic.RedCards);
        }

        private void StubMatchStatistic(string matchId, Match returnedMatch)
            => dynamicRepository.GetAsync<Match>(
                            Arg.Is<GetMatchByIdCriteria>(criteria => criteria.Id == matchId))
                            .Returns(Task.FromResult(returnedMatch));

        #endregion Match Statistic

        #region Match Lineups

        [Fact]
        public async Task GetMatchLineups_Always_ReturnDataFromCache()
        {
            var matchId = "cachedMatchId";
            var language = Language.en_US;
            StubRetunLineupsFromCache(matchId, language);

            var matchLineups = await matchQueryService.GetMatchLineups(matchId, language, default(DateTimeOffset));

            Assert.Equal(matchId, matchLineups.Id);
        }

        [Fact]
#pragma warning disable S2699 // Tests should include assertions
        public async Task GetMatchLineupsData_Always_ExecuteGetAsync()
#pragma warning restore S2699 // Tests should include assertions
        {
            var matchId = "matchid_testexecuteasync";

            await matchQueryService.GetMatchLineupsData(matchId, Language.en_US, default(DateTimeOffset));

            await dynamicRepository.Received(1).GetAsync<MatchLineups>(Arg.Is<GetMatchLineupsCriteria>(criteria => criteria.MatchId == matchId));
        }

        [Fact]
        public async Task GetMatchLineupsData_ForHomeStartingPlayers_ReturnNormalGoalStatistic()
        {
            var matchId = "GetMatchLineupsData_ForHomeStartingPlayers_ReturnNormalGoalStatistic";
            StubTimelineEvents(matchId);
            StubMatchLineups(matchId);

            var matchLineups = await matchQueryService.GetMatchLineupsData(matchId, Language.en_US, default(DateTimeOffset));

            var player = matchLineups.Home.Players.FirstOrDefault(pl => pl.Id == Player7ID);
            Assert.Equal(2, player.EventStatistic[EventType.ScoreChange]);
        }

        [Fact]
        public async Task GetMatchLineupsData_ForHomeStartingPlayers_ReturnOwnGoalStatistic()
        {
            var matchId = "GetMatchLineupsData_ForHomeStartingPlayers_ReturnOwnGoalStatistic";
            StubTimelineEvents(matchId);
            StubMatchLineups(matchId);

            var matchLineups = await matchQueryService.GetMatchLineupsData(matchId, Language.en_US, default(DateTimeOffset));

            var player = matchLineups.Home.Players.FirstOrDefault(pl => pl.Id == Player5ID);
            Assert.Equal(2, player.EventStatistic[EventType.ScoreChangeByOwnGoal]);
        }

        [Fact]
        public async Task GetMatchLineupsData_ForHomeStartingPlayers_ReturnPenaltyGoalStatistic()
        {
            var matchId = "GetMatchLineupsData_ForHomeStartingPlayers_ReturnPenaltyGoalStatistic";
            StubTimelineEvents(matchId);
            StubMatchLineups(matchId);

            var matchLineups = await matchQueryService.GetMatchLineupsData(matchId, Language.en_US, default(DateTimeOffset));

            var player = matchLineups.Home.Players.FirstOrDefault(pl => pl.Id == Player6ID);
            Assert.Equal(1, player.EventStatistic[EventType.ScoreChangeByPenalty]);
        }

        [Fact]
        public async Task GetMatchLineupsData_ForHomeStartingPlayers_ReturnYellowCardStatistic()
        {
            var matchId = "GetMatchLineupsData_ForHomeStartingPlayers_ReturnYellowCardStatistic";
            StubTimelineEvents(matchId);
            StubMatchLineups(matchId);

            var matchLineups = await matchQueryService.GetMatchLineupsData(matchId, Language.en_US, default(DateTimeOffset));

            var player = matchLineups.Home.Players.FirstOrDefault(pl => pl.Id == Player7ID);
            Assert.Equal(1, player.EventStatistic[EventType.YellowCard]);
        }

        [Fact]
        public async Task GetMatchLineupsData_ForHomeStartingPlayers_ReturnYellowRedCardStatistic()
        {
            var matchId = "GetMatchLineupsData_ForHomeStartingPlayers_ReturnYellowRedCardStatistic";
            StubTimelineEvents(matchId);
            StubMatchLineups(matchId);

            var matchLineups = await matchQueryService.GetMatchLineupsData(matchId, Language.en_US, default(DateTimeOffset));

            var player = matchLineups.Home.Players.FirstOrDefault(pl => pl.Id == Player7ID);
            Assert.Equal(1, player.EventStatistic[EventType.YellowRedCard]);
        }

        [Fact]
        public async Task GetMatchLineupsData_ForHomeStartingPlayers_ReturnRedCardStatistic()
        {
            var matchId = "GetMatchLineupsData_ForHomeStartingPlayers_ReturnRedCardStatistic";
            StubTimelineEvents(matchId);
            StubMatchLineups(matchId);

            var matchLineups = await matchQueryService.GetMatchLineupsData(matchId, Language.en_US, default(DateTimeOffset));

            var player = matchLineups.Home.Players.FirstOrDefault(pl => pl.Id == Player7ID);
            Assert.Equal(1, player.EventStatistic[EventType.RedCard]);
        }

        [Fact]
        public async Task GetMatchLineupsData_ForHomeStartingPlayers_ReturnSubtitutionEventWithJerseyNumber()
        {
            var matchId = "GetMatchLineupsData_ForHomeStartingPlayers_ReturnSubtitutionEventWithJerseyNumber";
            StubTimelineEvents(matchId);
            StubMatchLineups(matchId);

            var matchLineups = await matchQueryService.GetMatchLineupsData(matchId, Language.en_US, default(DateTimeOffset));

            var homeSubstitutionEvents = matchLineups.Home.SubstitutionEvents;

            Assert.Equal(2, homeSubstitutionEvents.Count());

            var playerIn = homeSubstitutionEvents.First().PlayerIn;
            Assert.Equal(Player1ID, playerIn.Id);
            Assert.Equal(1, playerIn.JerseyNumber);

            var playerOut = homeSubstitutionEvents.First().PlayerOut;
            Assert.Equal(Player2ID, playerOut.Id);
            Assert.Equal(2, playerOut.JerseyNumber);
        }

        [Fact]
        public async Task GetMatchLineupsData_ForHomeSubtitutePlayers_ReturnNormalGoalStatistic()
        {
            var matchId = "GetMatchLineupsData_ForHomeSubtitutePlayers_ReturnNormalGoalStatistic";
            StubTimelineEvents(matchId);
            StubMatchLineups(matchId);

            var matchLineups = await matchQueryService.GetMatchLineupsData(matchId, Language.en_US, default(DateTimeOffset));

            var player = matchLineups.Home.Substitutions.FirstOrDefault(pl => pl.Id == Player1ID);
            Assert.Equal(1, player.EventStatistic[EventType.ScoreChange]);
        }

        [Fact]
        public async Task GetMatchLineupsData_ForHomeSubtitutePlayers_ReturnYellowCardStatistic()
        {
            var matchId = "GetMatchLineupsData_ForHomeSubtitutePlayers_ReturnYellowCardStatistic";
            StubTimelineEvents(matchId);
            StubMatchLineups(matchId);

            var matchLineups = await matchQueryService.GetMatchLineupsData(matchId, Language.en_US, default(DateTimeOffset));

            var player = matchLineups.Home.Substitutions.FirstOrDefault(pl => pl.Id == Player1ID);
            Assert.Equal(1, player.EventStatistic[EventType.YellowCard]);
        }

        [Fact]
        public async Task GetMatchLineupsData_ForAwayStartingPlayers_ReturnYellowCardStatistic()
        {
            var matchId = "GetMatchLineupsData_ForHomeSubtitutePlayers_ReturnYellowCardStatistic";
            StubTimelineEvents(matchId);
            StubMatchLineups(matchId);

            var matchLineups = await matchQueryService.GetMatchLineupsData(matchId, Language.en_US, default(DateTimeOffset));

            var player = matchLineups.Away.Players.FirstOrDefault(pl => pl.Id == Player8ID);
            Assert.Equal(1, player.EventStatistic[EventType.YellowCard]);
        }

        private void StubRetunLineupsFromCache(
            string matchId = "matchId",
            Language language = null,
            MatchLineups matchLineups = null)
        {
            matchLineups = matchLineups ?? StubMatchLineups(matchId);
            language = language ?? Language.en_US;

            cacheManager.GetOrSetAsync(
               $"MatchQuery_MatchLineupCacheKey_{matchId}_{language.Value}",
               Arg.Any<Func<Task<MatchLineups>>>(),
               Arg.Any<CacheItemOptions>()).Returns(Task.FromResult(matchLineups));
        }

        private const string Player1ID = "1";
        private const string Player2ID = "2";
        private const string Player3ID = "3";
        private const string Player4ID = "4";
        private const string Player5ID = "5";
        private const string Player6ID = "6";
        private const string Player7ID = "7";
        private const string Player8ID = "8";
        private const string Player9ID = "9";
        private const string Player10ID = "10";

        private void StubTimelineEvents(string matchId, IEnumerable<TimelineEvent> timelineEvents = null)
        {
            
            timelineEvents = timelineEvents
                ?? new List<TimelineEvent>
                {
                    A.Dummy<TimelineEvent>()
                        .With(t => t.Team, "home")
                        .With(t => t.Type, EventType.Substitution)
                        .With(t => t.Time, new DateTimeOffset(new DateTime(2019, 2, 2)))
                        .With(t => t.PlayerIn, new Player(Player1ID, "Player 1", 2))
                        .With(t => t.PlayerOut, new Player(Player2ID, "Player 2", 12)),
                    A.Dummy<TimelineEvent>()
                        .With(t => t.Team, "home")
                        .With(t => t.Type, EventType.Substitution)
                        .With(t => t.Time, new DateTimeOffset(new DateTime(2019, 1, 2)))
                        .With(t => t.PlayerIn, new Player(Player3ID, "Player 3", 4))
                        .With(t => t.PlayerOut, new Player(Player4ID, "Player 4", 3)),
                    A.Dummy<TimelineEvent>()
                        .With(t => t.Team, "home")
                        .With(t => t.Type, EventType.ScoreChange)
                        .With(t => t.GoalScorer, new GoalScorer(Player5ID, "Player 5", "own_goal")),
                    A.Dummy<TimelineEvent>()
                        .With(t => t.Team, "home")
                        .With(t => t.Type, EventType.ScoreChange)
                        .With(t => t.GoalScorer, new GoalScorer(Player5ID, "Player 5", "own_goal")),
                    A.Dummy<TimelineEvent>()
                        .With(t => t.Team, "home")
                        .With(t => t.Type, EventType.ScoreChange)
                        .With(t => t.GoalScorer, new GoalScorer(Player6ID, "Player 6", "penalty")),
                    A.Dummy<TimelineEvent>()
                        .With(t => t.Team, "home")
                        .With(t => t.Type, EventType.ScoreChange)
                        .With(t => t.GoalScorer, new GoalScorer(Player7ID, "Player 7", string.Empty)),
                    A.Dummy<TimelineEvent>()
                        .With(t => t.Team, "home")
                        .With(t => t.Type, EventType.ScoreChange)
                        .With(t => t.GoalScorer, new GoalScorer(Player7ID, "Player 7", string.Empty)),
                    A.Dummy<TimelineEvent>()
                        .With(t => t.Team, "home")
                        .With(t => t.Type, EventType.YellowCard)
                        .With(t => t.Player, new Player(Player7ID, "Player 7")),
                    A.Dummy<TimelineEvent>()
                        .With(t => t.Team, "home")
                        .With(t => t.Type, EventType.YellowRedCard)
                        .With(t => t.Player, new Player(Player7ID, "Player 7")),
                    A.Dummy<TimelineEvent>()
                        .With(t => t.Team, "home")
                        .With(t => t.Type, EventType.RedCard)
                        .With(t => t.Player, new Player(Player7ID, "Player 7")),
                    A.Dummy<TimelineEvent>()
                        .With(t => t.Team, "home")
                        .With(t => t.Type, EventType.ScoreChange)
                        .With(t => t.GoalScorer, new GoalScorer(Player1ID, "Player 1", string.Empty)),
                    A.Dummy<TimelineEvent>()
                        .With(t => t.Team, "home")
                        .With(t => t.Type, EventType.YellowCard)
                        .With(t => t.Player, new Player(Player1ID, "Player 1")),
                    A.Dummy<TimelineEvent>()
                        .With(t => t.Team, "away")
                        .With(t => t.Type, EventType.YellowCard)
                        .With(t => t.Player, new Player(Player8ID, "Player 1")),
                };

            var match = fixture.Create<Match>().With(match => match.TimeLines, timelineEvents);

            dynamicRepository
                .GetAsync<Match>(Arg.Is<GetMatchTimelineEventsCriteria>(criteria => criteria.MatchId == matchId))
                .Returns(Task.FromResult(match));
        }

        private MatchLineups StubMatchLineups(string matchId, TeamLineups homeTeam = null, TeamLineups awayTeam = null)
        {
            var matchLineups = new MatchLineups(
                matchId,
                new DateTimeOffset(new DateTime(1989, 5, 28)),
                homeTeam ?? StubHomeLineups(),
                awayTeam ?? StubAwayLineups());

            dynamicRepository
                .GetAsync<MatchLineups>(Arg.Is<GetMatchLineupsCriteria>(criteria
                    => criteria.MatchId == matchId
                        && criteria.Language == Language.en_US.DisplayName))
                .Returns(Task.FromResult(matchLineups));

            return matchLineups;
        }

        private TeamLineups StubHomeLineups()
            => new TeamLineups(
                "homeid",
                "homename",
                true,
                new Coach("coachid", "coachname", "national", "country code"),
                "1-1",
                new List<Player>
                {
                    new Player(Player2ID, "player2 name", PlayerType.Midfielder, 2, Position.CentralMidfielder, 2),
                    new Player(Player4ID, "player4 name", PlayerType.Goalkeeper, 4, Position.CentralMidfielder, 1),
                    new Player(Player5ID, "player5 name", PlayerType.Defender, 5, Position.CentralDefender, 2),
                    new Player(Player6ID, "player6 name", PlayerType.Forward, 6, Position.RightWinger, 3),
                    new Player(Player7ID, "player7 name", PlayerType.Forward, 7, Position.Striker, 3)
                },
                new List<Player>
                {
                    new Player(Player1ID, "player1 name", PlayerType.Goalkeeper, 1, Position.Goalkeeper, 1),
                    new Player(Player3ID, "player3 name", PlayerType.Forward, 3, Position.Striker, 3)
                });

        private TeamLineups StubAwayLineups()
            => new TeamLineups(
                "awayid",
                "awayname",
                true,
                new Coach("coachid", "coachname", "national", "country code"),
                "2-0",
                new List<Player>
                {
                    new Player(Player8ID, "player1 name", PlayerType.Goalkeeper, 1, Position.Goalkeeper, 1),
                    new Player(Player9ID, "player2 name", PlayerType.Midfielder, 2, Position.CentralMidfielder, 2),
                    new Player(Player10ID, "player3 name", PlayerType.Forward, 3, Position.Striker, 3),
                    new Player(Player4ID, "player4 name", PlayerType.Goalkeeper, 4, Position.Striker, 1),
                    new Player(Player5ID, "player5 name", PlayerType.Defender, 5, Position.LeftBack, 2),
                    new Player(Player6ID, "player6 name", PlayerType.Forward, 6, Position.RightWinger, 3)
                },
                new List<Player>());

        #endregion Match Lineups
    }
}
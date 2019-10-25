using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Fanex.Caching;
using Fanex.Data.Repository;
using NSubstitute;
using Score247.Shared;
using Score247.Shared.Tests;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Matches.Models;
using Soccer.EventProcessors.Leagues.Filters;
using Xunit;

namespace Soccer.EventProcessors.Tests.Leagues
{
    [Trait("Soccer.EventProcessors", "LeagueFilter")]
    public class LeagueFilterTests
    {
        private static readonly Fixture fixture = new Fixture();
        private const string MajorLeaguesCacheKey = "Major_Leagues";
        private readonly ICacheManager cacheService;
        private readonly IMajorLeagueFilter<IEnumerable<Match>, IEnumerable<Match>> matchListFilter;
        private readonly IMajorLeagueFilter<Match, bool> matchFilter;
        private readonly IMajorLeagueFilter<MatchEvent, bool> matchEventFilter;

        public LeagueFilterTests()
        {
            var dynamicRepository = Substitute.For<IDynamicRepository>();
            cacheService = Substitute.For<ICacheManager>();

            matchListFilter = new MajorLeagueFilter(dynamicRepository, cacheService);
            matchFilter = new MajorLeagueFilter(dynamicRepository, cacheService);
            matchEventFilter = new MajorLeagueFilter(dynamicRepository, cacheService);
        }

        [Fact]
        public async Task FilterAsync_NoMatchInMajorLeague_ShouldReturnEmpty()
        {
            cacheService.GetOrSetAsync(MajorLeaguesCacheKey, Arg.Any<Func<Task<IEnumerable<League>>>>(), Arg.Any<CacheItemOptions>())
                .Returns(new List<League> {
                    new League("league:1", ""),
                    new League("league:2", "")
                });

            var matches = new List<Match>
            {
                fixture.For<Match>()
                    .With(m => m.Id, "match:1")
                    .With(m => m.League, new League("league:3", ""))
                    .Create(),
                fixture.For<Match>()
                    .With(m => m.Id, "match:2")
                    .With(m => m.League, new League("league:4", ""))
                    .Create(),
            };

            var filteredMatches = await matchListFilter.Filter(matches);

            Assert.Empty(filteredMatches);
        }

        [Fact]
        public async Task FilterAsync_MatchInMajorLeague_ShouldReturnFilteredMatch()
        {
            cacheService.GetOrSetAsync(MajorLeaguesCacheKey, Arg.Any<Func<Task<IEnumerable<League>>>>(), Arg.Any<CacheItemOptions>())
                .Returns(new List<League> {
                    new League("league:1", ""),
                    new League("league:2", "")
                });

            var matches = new List<Match>
            {
                fixture.For<Match>()
                    .With(m => m.Id, "match:1")
                    .With(m => m.League, new League("league:3", ""))
                    .Create(),
                fixture.For<Match>()
                    .With(m => m.Id, "match:2")
                    .With(m => m.League, new League("league:4", ""))
                    .Create(),
                fixture.For<Match>()
                    .With(m => m.Id, "match:3")
                    .With(m => m.League, new League("league:1", ""))
                    .Create(),
                fixture.For<Match>()
                    .With(m => m.Id, "match:5")
                    .With(m => m.League, new League("league:2", ""))
                    .Create(),
                fixture.For<Match>()
                    .With(m => m.Id, "match:4")
                    .With(m => m.League, new League("league:6", ""))
                    .Create()
            };

            var filteredMatches = (await matchListFilter.Filter(matches)).ToList();

            Assert.Equal(2, filteredMatches.Count);
            Assert.Contains(filteredMatches, m => m.Id == "match:3");
            Assert.Contains(filteredMatches, m => m.Id == "match:5");
        }

        [Fact]
        public async Task Filter_MatchIsNull_ShouldReturnFalse()
        {
            cacheService.GetOrSetAsync(MajorLeaguesCacheKey, Arg.Any<Func<Task<IEnumerable<League>>>>(), Arg.Any<CacheItemOptions>())
                .Returns(new List<League> {
                    new League("league:1", ""),
                    new League("league:2", "")
                });

            var result = (await matchFilter.Filter(null));

            Assert.False(result);
        }

        [Fact]
        public async Task Filter_MatchNotInMajorLeague_ShouldReturnFalse()
        {
            cacheService.GetOrSetAsync(MajorLeaguesCacheKey, Arg.Any<Func<Task<IEnumerable<League>>>>(), Arg.Any<CacheItemOptions>())
                .Returns(new List<League> {
                    new League("league:1", ""),
                    new League("league:2", "")
                });

            var match = fixture.For<Match>()
                .With(m => m.Id, "match:1")
                .With(m => m.League, new League("league:6", ""))
                .Create();

            var result = (await matchFilter.Filter(match));

            Assert.False(result);
        }

        [Fact]
        public async Task FilterAsync_MatchInMajorLeague_ShouldReturnTrue()
        {
            cacheService.GetOrSetAsync(MajorLeaguesCacheKey, Arg.Any<Func<Task<IEnumerable<League>>>>(), Arg.Any<CacheItemOptions>())
                .Returns(new List<League> {
                    new League("league:1", ""),
                    new League("league:2", "")
                });

            var match = fixture.For<Match>()
                .With(m => m.Id, "match:1")
                .With(m => m.League, new League("league:1", ""))
                .Create();

            var result = (await matchFilter.Filter(match));

            Assert.True(result);
        }

        [Fact]
        public async Task Filter_MatchEventIsNull_ShouldReturnFalse()
        {
            cacheService.GetOrSetAsync(MajorLeaguesCacheKey, Arg.Any<Func<Task<IEnumerable<League>>>>(), Arg.Any<CacheItemOptions>())
                .Returns(new List<League> {
                    new League("league:1", ""),
                    new League("league:2", "")
                });

            var result = (await matchEventFilter.Filter(null));

            Assert.False(result);
        }

        [Fact]
        public async Task Filter_MatchEventNotInMajorLeague_ShouldReturnFalse()
        {
            cacheService.GetOrSetAsync(MajorLeaguesCacheKey, Arg.Any<Func<Task<IEnumerable<League>>>>(), Arg.Any<CacheItemOptions>())
                .Returns(new List<League> {
                    new League("league:1", ""),
                    new League("league:2", "")
                });

            var match = new MatchEvent("league:3", "match:1", null, null);

            var result = (await matchEventFilter.Filter(match));

            Assert.False(result);
        }

        [Fact]
        public async Task FilterAsync_MatchEventInMajorLeague_ShouldReturnTrue()
        {
            cacheService.GetOrSetAsync(MajorLeaguesCacheKey, Arg.Any<Func<Task<IEnumerable<League>>>>(), Arg.Any<CacheItemOptions>())
                .Returns(new List<League> {
                    new League("league:1", ""),
                    new League("league:2", "")
                });

            var match = new MatchEvent("league:1", "match:1", null, null);

            var result = (await matchEventFilter.Filter(match));

            Assert.True(result);
        }
    }
}
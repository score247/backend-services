using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Caching;
using Fanex.Data.Repository;
using NSubstitute;
using Score247.Shared;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Matches.Models;
using Soccer.EventProcessors._Shared.Filters;
using Soccer.EventProcessors.Leagues;
using Xunit;

namespace Soccer.EventProcessors.Tests.Leagues
{
    [Trait("Soccer.EventProcessors", "LeagueFilter")]
    public class LeagueFilterTests
    {
        private const string MajorLeaguesCacheKey = "Major_Leagues";
        private readonly IDynamicRepository dynamicRepository;
        private readonly ICacheManager cacheService;
        private readonly IAsyncFilter<IEnumerable<Match>, IEnumerable<Match>> matchListFilter;
        private readonly IAsyncFilter<Match, bool> matchFilter;
        private readonly IAsyncFilter<MatchEvent, bool> matchEventFilter;

        public LeagueFilterTests()
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            cacheService = Substitute.For<ICacheManager>();

            matchListFilter = new LeagueFilter(dynamicRepository, cacheService);
            matchFilter = new LeagueFilter(dynamicRepository, cacheService);
            matchEventFilter = new LeagueFilter(dynamicRepository, cacheService);
        }

        [Fact]
        public async Task FilterAsync_NoMatchInMajorLeague_ShouldReturnEmpty()
        {
            cacheService.GetOrSetAsync(MajorLeaguesCacheKey, Arg.Any<Func<Task<IEnumerable<League>>>>(), Arg.Any<CacheItemOptions>())
                .Returns(new List<League> {
                    new League{ Id = "league:1" },
                    new League{ Id = "league:2" }
                });

            var matches = new List<Match>
            {
                new Match { Id = "match:1", League = new League{ Id = "league:3" }  },
                new Match { Id = "match:2", League = new League{ Id = "league:4" }  }
            };

            var filteredMatches = await matchListFilter.Filter(matches);

            Assert.Empty(filteredMatches);
        }

        [Fact]
        public async Task FilterAsync_MatchInMajorLeague_ShouldReturnFilteredMatch()
        {
            cacheService.GetOrSetAsync(MajorLeaguesCacheKey, Arg.Any<Func<Task<IEnumerable<League>>>>(), Arg.Any<CacheItemOptions>())
                .Returns(new List<League> {
                    new League{ Id = "league:1" },
                    new League{ Id = "league:2" }
                });

            var matches = new List<Match>
            {
                new Match { Id = "match:1", League = new League{ Id = "league:3" }  },
                new Match { Id = "match:2", League = new League{ Id = "league:4" }  },
                new Match { Id = "match:3", League = new League{ Id = "league:1" }  },
                new Match { Id = "match:5", League = new League{ Id = "league:2" }  },
                new Match { Id = "match:4", League = new League{ Id = "league:6" }  }
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
                    new League{ Id = "league:1" },
                    new League{ Id = "league:2" }
                });

            var result = (await matchFilter.Filter(null));

            Assert.False(result);
        }

        [Fact]
        public async Task Filter_MatchNotInMajorLeague_ShouldReturnFalse()
        {
            cacheService.GetOrSetAsync(MajorLeaguesCacheKey, Arg.Any<Func<Task<IEnumerable<League>>>>(), Arg.Any<CacheItemOptions>())
                .Returns(new List<League> {
                    new League{ Id = "league:1" },
                    new League{ Id = "league:2" }
                });

            var match = new Match { Id = "match:1", League = new League { Id = "league:3" } };

            var result = (await matchFilter.Filter(match));

            Assert.False(result);
        }

        [Fact]
        public async Task FilterAsync_MatchInMajorLeague_ShouldReturnTrue()
        {
            cacheService.GetOrSetAsync(MajorLeaguesCacheKey, Arg.Any<Func<Task<IEnumerable<League>>>>(), Arg.Any<CacheItemOptions>())
                .Returns(new List<League> {
                    new League{ Id = "league:1" },
                    new League{ Id = "league:2" }
                });

            var match = new Match { Id = "match:1", League = new League { Id = "league:1" } };

            var result = (await matchFilter.Filter(match));

            Assert.True(result);
        }

        [Fact]
        public async Task Filter_MatchEventIsNull_ShouldReturnFalse()
        {
            cacheService.GetOrSetAsync(MajorLeaguesCacheKey, Arg.Any<Func<Task<IEnumerable<League>>>>(), Arg.Any<CacheItemOptions>())
                .Returns(new List<League> {
                    new League{ Id = "league:1" },
                    new League{ Id = "league:2" }
                });

            var result = (await matchEventFilter.Filter(null));

            Assert.False(result);
        }

        [Fact]
        public async Task Filter_MatchEventNotInMajorLeague_ShouldReturnFalse()
        {
            cacheService.GetOrSetAsync(MajorLeaguesCacheKey, Arg.Any<Func<Task<IEnumerable<League>>>>(), Arg.Any<CacheItemOptions>())
                .Returns(new List<League> {
                    new League{ Id = "league:1" },
                    new League{ Id = "league:2" }
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
                    new League{ Id = "league:1" },
                    new League{ Id = "league:2" }
                });

            var match = new MatchEvent("league:1", "match:1", null, null);

            var result = (await matchEventFilter.Filter(match));

            Assert.True(result);
        }
    }
}

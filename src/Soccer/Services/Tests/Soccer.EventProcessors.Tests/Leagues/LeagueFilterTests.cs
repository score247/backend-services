using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Caching;
using Fanex.Data.Repository;
using NSubstitute;
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
        private readonly ICacheService cacheService;
        private readonly IFilter<IEnumerable<Match>, IEnumerable<Match>> leagueFilter;

        public LeagueFilterTests()
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            cacheService = Substitute.For<ICacheService>();

            leagueFilter = new LeagueFilter(dynamicRepository, cacheService);
        }

        [Fact]
        public async Task FilterAsync_NoMatchInMajorLeague_ShouldReturnEmpty()
        {
            cacheService.GetAsync<IEnumerable<League>>(MajorLeaguesCacheKey)
                .Returns(new List<League> {
                    new League{ Id = "league:1" },
                    new League{ Id = "league:2" }
                });

            var matches = new List<Match>
            {
                new Match { Id = "match:1", League = new League{ Id = "league:3" }  },
                new Match { Id = "match:2", League = new League{ Id = "league:4" }  }
            };

            var filteredMatches = await leagueFilter.FilterAsync(matches);

            Assert.Empty(filteredMatches);
        }

        [Fact]
        public async Task FilterAsync_MatchInMajorLeague_ShouldReturnFilteredMatch()
        {
            cacheService.GetAsync<IEnumerable<League>>(MajorLeaguesCacheKey)
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

            var filteredMatches = (await leagueFilter.FilterAsync(matches)).ToList();

            Assert.Equal(2, filteredMatches.Count);
            Assert.Contains(filteredMatches, m => m.Id == "match:3");
            Assert.Contains(filteredMatches, m => m.Id == "match:5");
        }
    }
}

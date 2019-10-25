using System;
using System.Threading.Tasks;
using Fanex.Caching;
using Fanex.Data.Repository;
using NSubstitute;
using Score247.Shared;
using Soccer.API.Matches;
using Soccer.API.Shared.Configurations;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.Database.Timelines.Criteria;
using Xunit;

namespace Soccer.API.Tests.Matches
{
    [Trait("Soccer.API", "Match")]
    public class MatchQueryServiceTests
    {
        private readonly MatchQueryService matchQueryService;
        private readonly IDynamicRepository dynamicRepository;

        public MatchQueryServiceTests()
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            var appSettings = Substitute.For<IAppSettings>();
            matchQueryService = new MatchQueryService(dynamicRepository, new CacheManager(new CacheService()), appSettings, () => DateTimeOffset.Now);
        }

        [Fact]
        public async Task GetMatchCommentary_ExecuteFetchAsync()
        {
            await matchQueryService.GetMatchCommentary("sr:match:1", Language.en_US);

            var results = await dynamicRepository.Received(1).FetchAsync<TimelineEvent>(Arg.Any<GetCommentaryCriteria>());

            Assert.Empty(results);
        }
    }
}
namespace Soccer.API.Tests.Matches
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using NSubstitute;
    using Soccer.API._Shared;
    using Soccer.API.Matches;
    using Soccer.API.Shared.Configurations;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Shared.Enumerations;
    using Soccer.Database.Timelines.Criteria;
    using Xunit;

    [Trait("Soccer.API", "Match")]
    public class MatchQueryServiceTests
    {
        private readonly MatchQueryService matchQueryService;
        private readonly IDynamicRepository dynamicRepository;
        private readonly ICacheManager cacheManager;
        private readonly IAppSettings appSettings;

        public MatchQueryServiceTests()
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            cacheManager = Substitute.For<ICacheManager>();
            appSettings = Substitute.For<IAppSettings>();

            matchQueryService = new MatchQueryService(dynamicRepository, cacheManager, appSettings, () => DateTimeOffset.Now);
        }

        [Fact]
        public async Task GetMatchCommentary_ExecuteFetchAsync() 
        {
            await matchQueryService.GetMatchCommentary("sr:match", Language.en_US);

            var results = await dynamicRepository.Received(1).FetchAsync<TimelineEvent>(Arg.Any<GetCommentaryCriteria>());

            Assert.Empty(results);
        }

        [Fact]
        public async Task GetMatchCommentary_ShouldOrderByTime()
        {
            dynamicRepository.FetchAsync<TimelineEvent>(Arg.Any<GetCommentaryCriteria>()).Returns(new List<TimelineEvent> { 
                new TimelineEvent { Id = "1", Time = DateTimeOffset.Now },
                new TimelineEvent { Id = "2", Time = DateTimeOffset.Now.AddMinutes(-1) }
            });

            var timelines = await matchQueryService.GetMatchCommentary("sr:match", Language.en_US);

            Assert.Equal("2", timelines.First().TimelineId);
        }
    }
}

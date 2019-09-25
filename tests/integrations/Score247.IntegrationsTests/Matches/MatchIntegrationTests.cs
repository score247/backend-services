namespace Score247.IntegrationsTests.Matches
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Refit;
    using Score247.IntegrationTests;
    using Soccer.Core.Shared.Enumerations;
    using Xunit;

    public class MatchIntegrationTests
    {
        private readonly ISoccerMatchApi soccerMatchApi;

        public MatchIntegrationTests()
        {
            var messagePackRefitSettings = new RefitSettings
            {
                ContentSerializer = new MessagePackContentSerializer()
            };
            soccerMatchApi = RestService.For<ISoccerMatchApi>("http://localhost:57392/api/", messagePackRefitSettings);
        }

        [Fact]
        public async Task TestLiveMatchAPIs()
        {
            var liveMatches = await soccerMatchApi.GetLiveMatches(Language.en_US.DisplayName);
            var liveMatchCount = await soccerMatchApi.GetLiveMatchCount(Language.en_US.DisplayName);

            Assert.True(true);
        }
    }
}

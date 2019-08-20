namespace Soccer.EventProcessors.Tests.Odds
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using MassTransit;
    using NSubstitute;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Odds.Models;
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

        public OddsChangeConsumerTests()
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            cacheService = Substitute.For<ICacheService>();
            messageBus = Substitute.For<IBus>();
            getCurrentTimeFunc = Substitute.For<Func<DateTime>>();
            appSettings = Substitute.For<IAppSettings>();
            oddsChangeConsumer = new OddsChangeConsumer(
                dynamicRepository,
                getCurrentTimeFunc,
                messageBus,
                cacheService,
                appSettings);
        }

        public async Task Consume_NoMatchExist_DoesNotExecuteGetOdds()
        {
            cacheService.GetOrSetAsync(
                Arg.Any<string>(),
                Arg.Any<Func<IEnumerable<Match>>>(),
                Arg.Any<CacheItemOptions>()).Returns(Enumerable.Empty<Match>());

            await dynamicRepository.DidNotReceive().FetchAsync<BetTypeOdds>(Arg.Any<GetOddsCriteria>());


        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using MassTransit;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Leagues.QueueMessages;
using Soccer.Core.Shared.Enumerations;
using Soccer.EventProcessors.Leagues.Services;

namespace Soccer.EventProcessors.Leagues
{
    public class FetchLeagueStandingConsumer : IConsumer<ILeagueStandingFetchedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly ILeagueService leagueService;

        public FetchLeagueStandingConsumer(IDynamicRepository dynamicRepository, ILeagueService leagueService)
        {
            this.dynamicRepository = dynamicRepository;
            this.leagueService = leagueService;
        }

        public async Task Consume(ConsumeContext<ILeagueStandingFetchedMessage> context)
        {
            var message = context.Message;
        }
    }
}
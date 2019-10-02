using System.Threading.Tasks;
using Fanex.Data.Repository;
using MassTransit;
using Soccer.Core.Leagues.QueueMessages;

namespace Soccer.EventProcessors.Leagues
{
    public class FetchGetSportradarLeaguesConsumer : IConsumer<ISyncSportradarLeaguesMessage>
    {
        private readonly IDynamicRepository dynamicRepository;

        public FetchGetSportradarLeaguesConsumer(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public Task Consume(ConsumeContext<ISyncSportradarLeaguesMessage> context)
        {
            throw new System.NotImplementedException();
        }
    }
}
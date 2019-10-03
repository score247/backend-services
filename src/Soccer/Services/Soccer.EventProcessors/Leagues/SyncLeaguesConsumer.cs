using System.Threading.Tasks;
using Fanex.Data.Repository;
using MassTransit;
using Soccer.Core.Leagues.QueueMessages;

namespace Soccer.EventProcessors.Leagues
{
    public class SyncLeaguesConsumer : IConsumer<ILeaguesSyncedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;

        public SyncLeaguesConsumer(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public Task Consume(ConsumeContext<ILeaguesSyncedMessage> context)
        {
            throw new System.NotImplementedException();
        }
    }
}
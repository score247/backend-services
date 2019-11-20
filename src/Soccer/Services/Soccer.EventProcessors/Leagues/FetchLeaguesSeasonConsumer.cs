using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using MassTransit;
using Soccer.Core.Leagues.QueueMessages;
using Soccer.Database.Leagues.Commands;

namespace Soccer.EventProcessors.Leagues
{
    public class FetchLeaguesSeasonConsumer : IConsumer<ILeaguesSeasonFetchedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;

        public FetchLeaguesSeasonConsumer(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task Consume(ConsumeContext<ILeaguesSeasonFetchedMessage> context)
        {
            var message = context.Message;

            if (message.Leagues?.Any() == false)
            {
                return;
            }

            var command = new InsertLeagueSeasonCommand(message.Leagues);

            await dynamicRepository.ExecuteAsync(command);
        }
    }
}

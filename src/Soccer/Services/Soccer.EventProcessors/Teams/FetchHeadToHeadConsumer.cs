using System;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using MassTransit;
using Soccer.Core.Teams.QueueMessages;
using Soccer.Database.Teams;

namespace Soccer.EventProcessors.Teams
{
    public class FetchHeadToHeadConsumer : IConsumer<IHeadToHeadFetchedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;

        public FetchHeadToHeadConsumer(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public Task Consume(ConsumeContext<IHeadToHeadFetchedMessage> context)
        {
            var message = context?.Message;

            if (message != null)
            {
                var headToHeads = message.HeadToHeads.ToList();

                var insertHeadToHeadTasks = headToHeads.Select(h
                    => dynamicRepository.ExecuteAsync(new InsertOrUpdateHeadToHeadCommand(message.HomeTeamId, message.AwayTeamId, h.Match)));

                return Task.WhenAll(insertHeadToHeadTasks);
            }

            return Task.CompletedTask;
        }
    }
}
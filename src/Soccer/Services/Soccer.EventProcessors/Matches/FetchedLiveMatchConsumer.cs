using System.Linq;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using MassTransit;
using Soccer.Core.Matches.Events;
using Soccer.Core.Matches.Models;
using Soccer.Core.Matches.QueueMessages;
using Soccer.Database.Matches.Commands;
using Soccer.Database.Matches.Criteria;

namespace Soccer.EventProcessors.Matches
{
    public class FetchedLiveMatchConsumer : IConsumer<ILiveMatchFetchedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly IBus messageBus;

        public FetchedLiveMatchConsumer(IBus messageBus, IDynamicRepository dynamicRepository)
        {
            this.messageBus = messageBus;
            this.dynamicRepository = dynamicRepository;
        }

        public async Task Consume(ConsumeContext<ILiveMatchFetchedMessage> context)
        {
            var message = context.Message;

            if (!message.Matches.Any())
            {
                return;
            }

            var currentLiveMatches = (await dynamicRepository
                    .FetchAsync<Match>(new GetLiveMatchesCriteria(message.Language)))
                    .ToList();
            var removedMatches = currentLiveMatches.Except(message.Matches).ToList();
            var newLiveMatches = message.Matches.Except(currentLiveMatches).ToList();

            if (removedMatches.Count > 0 || newLiveMatches.Count > 0)
            {
                var command = new InsertOrRemoveLiveMatchesCommand(message.Language, newLiveMatches, removedMatches);

                await dynamicRepository.ExecuteAsync(command);

                await messageBus.Publish<ILiveMatchUpdatedMessage>(new LiveMatchUpdatedMessage(message.Language, newLiveMatches, removedMatches));
            }
        }
    }
}
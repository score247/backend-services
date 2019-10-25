using System.Threading.Tasks;
using Fanex.Data.Repository;
using MassTransit;
using Soccer.Core.Matches.QueueMessages;
using Soccer.Database.Matches.Commands;

namespace Soccer.EventProcessors.Matches
{
    public class FetchMatchLineupsConsumer : IConsumer<IMatchLineupsMessage>
    {
        private readonly IDynamicRepository dynamicRepository;

        public FetchMatchLineupsConsumer(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task Consume(ConsumeContext<IMatchLineupsMessage> context)
        {
            var message = context.Message;

            if (message?.MatchLineups == null)
            {
                return;
            }

            var command = new InsertOrUpdateMatchLineupsCommand(message.MatchLineups, message.Language);

            await dynamicRepository.ExecuteAsync(command);
        }
    }
}
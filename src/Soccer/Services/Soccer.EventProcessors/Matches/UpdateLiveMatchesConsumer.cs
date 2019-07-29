namespace Soccer.EventProcessors.Matches
{
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using MassTransit;
    using Soccer.Core.Matches.Events;
    using Soccer.Database.Matches.Commands;

    public class UpdateLiveMatchesConsumer : IConsumer<ILiveMatchUpdatedToClosedEvent>
    {
        private readonly IDynamicRepository dynamicRepository;

        public UpdateLiveMatchesConsumer(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task Consume(ConsumeContext<ILiveMatchUpdatedToClosedEvent> context)
        {
            var message = context.Message;

            var command = new UpdateMatchResultCommand(message.MatchId, message.Result, message.Language);

            await dynamicRepository.ExecuteAsync(command);
        }
    }
}

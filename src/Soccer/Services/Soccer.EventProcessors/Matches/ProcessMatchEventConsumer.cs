namespace Soccer.EventProcessors.Matches
{
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using MassTransit;
    using Soccer.Core.Matches.QueueMessages;
    using Soccer.Database.Matches.Commands;

    public class ProcessMatchEventConsumer : IConsumer<IMatchEventProcessedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;

        public ProcessMatchEventConsumer(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task Consume(ConsumeContext<IMatchEventProcessedMessage> context)
        {
            var matchEvent = context?.Message?.MatchEvent;

            if (matchEvent != null)
            {
                // TODO : Execute parallel
                await dynamicRepository.ExecuteAsync(new InsertTimelineCommand(matchEvent.MatchId, matchEvent.Timeline));
                await dynamicRepository.ExecuteAsync(new UpdateLiveMatchResultCommand(matchEvent.MatchId, matchEvent.MatchResult));
            }
        }
    }
}
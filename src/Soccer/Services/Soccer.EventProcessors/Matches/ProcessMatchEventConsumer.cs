namespace Soccer.EventProcessors.Matches
{
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using MassTransit;
    using Score247.Shared.Enumerations;
    using Soccer.Core._Shared.Enumerations;
    using Soccer.Core.Matches.QueueMessages;
    using Soccer.Database.Matches.Commands;

    public class ProcessMatchEventConsumer : IConsumer<IMatchEventProcessed>
    {
        private readonly IDynamicRepository dynamicRepository;

        public ProcessMatchEventConsumer(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task Consume(ConsumeContext<IMatchEventProcessed> context)
        {
            var matchEvent = context?.Message?.MatchEvent;

            if (matchEvent != null)
            {
                // TODO : Execute parallel
                await dynamicRepository.ExecuteAsync(new InsertTimelineCommand(matchEvent.MatchId, matchEvent.Timeline));

                foreach (var language in Enumeration.GetAll<Language>())
                {
                    await dynamicRepository.ExecuteAsync(new UpdateMatchResultCommand(matchEvent.MatchId, matchEvent.MatchResult, language.DisplayName));
                }
            }
        }
    }
}
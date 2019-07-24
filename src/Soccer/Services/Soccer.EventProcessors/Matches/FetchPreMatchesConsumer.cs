namespace Soccer.EventProcessors.Matches
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;
    using Soccer.Core.Domain.Matches.Events;
    using Soccer.Core.Domain.Matches.Repositories;

    public class FetchPreMatchesConsumer : IConsumer<PreMatchesFetchedEvent>
    {
        private readonly IMatchRepository matchRepository;

        public FetchPreMatchesConsumer(IMatchRepository matchRepository)
        {
            this.matchRepository = matchRepository;
        }

        public async Task Consume(ConsumeContext<PreMatchesFetchedEvent> context)
        {
            var message = context.Message;

            await matchRepository.InsertOrUpdatePreMatches(message.Matches, message.Language);
        }
    }
}
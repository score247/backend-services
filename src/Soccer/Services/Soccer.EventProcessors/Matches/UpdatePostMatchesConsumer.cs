namespace Soccer.EventProcessors.Matches
{
    using System.Threading.Tasks;
    using MassTransit;
    using Soccer.Core.Domain.Matches.Events;
    using Soccer.Core.Domain.Matches.Repositories;

    public class UpdatePostMatchesConsumer : IConsumer<PostMatchUpdatedEvent>
    {
        private readonly IMatchRepository matchRepository;

        public UpdatePostMatchesConsumer(IMatchRepository matchRepository)
        {
            this.matchRepository = matchRepository;
        }

        public async Task Consume(ConsumeContext<PostMatchUpdatedEvent> context)
        {
            var message = context.Message;

            await matchRepository.InsertOrUpdateMatches(message.Matches, message.Language);
        }
    }
}
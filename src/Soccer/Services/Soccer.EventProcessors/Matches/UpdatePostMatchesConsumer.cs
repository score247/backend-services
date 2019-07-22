namespace Soccer.EventProcessors.Matches
{
    using System.Linq;
    using System.Threading.Tasks;
    using MassTransit;
    using Soccer.Core.Domain.Matches;
    using Soccer.Core.Domain.Matches.Entities;
    using Soccer.Core.Domain.Matches.Events;

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

            var matchEntities = message.Matches.Select(m => new MatchEntity { Match = m, Language = message.Language });

            await matchRepository.UpdateRange(matchEntities);
        }
    }
}
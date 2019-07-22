namespace Soccer.EventProcessors.Matches
{
    using System.Linq;
    using System.Threading.Tasks;
    using MassTransit;
    using Soccer.Core.Domain.Matches;
    using Soccer.Core.Domain.Matches.Entities;
    using Soccer.Core.Domain.Matches.Events;

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

            var matchEntities = message.Matches.Select(m => new MatchEntity { Match = m, Language = message.Language });

            await matchRepository.AddRange(matchEntities);
        }
    }
}
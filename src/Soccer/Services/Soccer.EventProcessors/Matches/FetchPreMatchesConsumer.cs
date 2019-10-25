using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using MassTransit;
using Soccer.Core.Matches.Events;
using Soccer.Core.Matches.Models;
using Soccer.Database.Matches.Commands;
using Soccer.EventProcessors.Leagues;
using Soccer.EventProcessors.Leagues.Filters;

namespace Soccer.EventProcessors.Matches
{
    public class FetchPreMatchesConsumer : IConsumer<IPreMatchesFetchedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly IMajorLeagueFilter<IEnumerable<Match>, IEnumerable<Match>> leagueFilter;

        public FetchPreMatchesConsumer(
            IDynamicRepository dynamicRepository,
            IMajorLeagueFilter<IEnumerable<Match>, IEnumerable<Match>> leagueFilter)
        {
            this.dynamicRepository = dynamicRepository;
            this.leagueFilter = leagueFilter;
        }

        public async Task Consume(ConsumeContext<IPreMatchesFetchedMessage> context)
        {
            var message = context.Message;
            var filteredMatches = (await leagueFilter.Filter(message.Matches))
                                    .ToList();

            var command = new InsertOrUpdateMatchesCommand(filteredMatches, message.Language);

            await dynamicRepository.ExecuteAsync(command);
        }
    }
}
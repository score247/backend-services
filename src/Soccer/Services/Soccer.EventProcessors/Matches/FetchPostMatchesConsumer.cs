namespace Soccer.EventProcessors.Matches
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using MassTransit;
    using Soccer.Core.Matches.Events;
    using Soccer.Core.Matches.Models;
    using Soccer.Database.Matches.Commands;
    using Soccer.EventProcessors._Shared.Filters;
    using Soccer.EventProcessors.Leagues;

    public class FetchPostMatchesConsumer : IConsumer<IPostMatchFetchedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly IAsyncFilter<IEnumerable<Match>, IEnumerable<Match>> leagueFilter;

        public FetchPostMatchesConsumer(
            IDynamicRepository dynamicRepository,
            IAsyncFilter<IEnumerable<Match>, IEnumerable<Match>> leagueFilter)
        {
            this.dynamicRepository = dynamicRepository;
            this.leagueFilter = leagueFilter;
        }

        public async Task Consume(ConsumeContext<IPostMatchFetchedMessage> context)
        {
            var message = context.Message;
            var filteredMatches = (await leagueFilter.Filter(message.Matches))
                                    .ToList();

            var command = new InsertOrUpdatePostMatchesCommand(filteredMatches, message.Language);

            await dynamicRepository.ExecuteAsync(command);
        }
    }
}
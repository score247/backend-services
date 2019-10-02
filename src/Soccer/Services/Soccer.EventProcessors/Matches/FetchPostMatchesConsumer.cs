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
        private readonly IFilter<IEnumerable<Match>, IEnumerable<Match>> leagueFilter;
        private readonly ILeagueGenerator leagueGenerator;

        public FetchPostMatchesConsumer(
            IDynamicRepository dynamicRepository,
            IFilter<IEnumerable<Match>, IEnumerable<Match>> leagueFilter,
            ILeagueGenerator leagueGenerator)
        {
            this.dynamicRepository = dynamicRepository;
            this.leagueFilter = leagueFilter;
            this.leagueGenerator = leagueGenerator;
        }

        public async Task Consume(ConsumeContext<IPostMatchFetchedMessage> context)
        {
            var message = context.Message;
            var filteredMatches = (await leagueFilter.FilterAsync(message.Matches))
                                    .Select(match => leagueGenerator.GenerateInternationalCode(match))
                                    .ToList();

            var command = new InsertOrUpdatePostMatchesCommand(filteredMatches, message.Language);

            await dynamicRepository.ExecuteAsync(command);
        }
    }
}
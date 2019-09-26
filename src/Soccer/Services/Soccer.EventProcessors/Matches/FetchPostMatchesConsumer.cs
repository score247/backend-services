namespace Soccer.EventProcessors.Matches
{
    using Fanex.Data.Repository;
    using MassTransit;
    using Soccer.Core.Matches.Events;
    using Soccer.Core.Matches.Models;
    using Soccer.Database.Matches.Commands;
    using Soccer.EventProcessors._Shared.Filters;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class FetchPostMatchesConsumer : IConsumer<IPostMatchFetchedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly IFilter<IEnumerable<Match>> leagueFilter;

        public FetchPostMatchesConsumer(IDynamicRepository dynamicRepository, IFilter<IEnumerable<Match>> leagueFilter)
        {
            this.dynamicRepository = dynamicRepository;
            this.leagueFilter = leagueFilter;
        }

        public async Task Consume(ConsumeContext<IPostMatchFetchedMessage> context)
        {
            var message = context.Message;

            var filterdMatches = await leagueFilter.FilterAsync(message.Matches);
            var command = new InsertOrUpdatePostMatchesCommand(filterdMatches, message.Language);

            await dynamicRepository.ExecuteAsync(command);
        }
    }
}
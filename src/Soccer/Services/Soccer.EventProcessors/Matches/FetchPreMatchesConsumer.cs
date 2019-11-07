using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using MassTransit;
using Soccer.Core.Leagues.Extensions;
using Soccer.Core.Matches.Events;
using Soccer.Core.Matches.Models;
using Soccer.Database.Matches.Commands;
using Soccer.EventProcessors.Leagues.Filters;
using Soccer.EventProcessors.Leagues.Services;

namespace Soccer.EventProcessors.Matches
{
    public class FetchPreMatchesConsumer : IConsumer<IPreMatchesFetchedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly IMajorLeagueFilter<IEnumerable<Match>, IEnumerable<Match>> leagueFilter;
        private readonly ILeagueService leagueService;

        public FetchPreMatchesConsumer(
            IDynamicRepository dynamicRepository,
            IMajorLeagueFilter<IEnumerable<Match>, IEnumerable<Match>> leagueFilter,
            ILeagueService leagueService)
        {
            this.dynamicRepository = dynamicRepository;
            this.leagueFilter = leagueFilter;
            this.leagueService = leagueService;
        }

        public async Task Consume(ConsumeContext<IPreMatchesFetchedMessage> context)
        {
            var message = context.Message;
            var majorLeagues = await leagueService.GetMajorLeagues();
            var filteredMatches = (await leagueFilter.Filter(message.Matches))
                .Select(match =>
                {
                    match.League.UpdateMajorLeagueInfo(majorLeagues);

                    return match;
                });

            var command = new InsertOrUpdateMatchesCommand(filteredMatches, message.Language);

            await dynamicRepository.ExecuteAsync(command);
        }
    }
}
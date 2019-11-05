using System.Linq;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using MassTransit;
using Soccer.Core.Matches.Models;
using Soccer.Core.Teams.QueueMessages;
using Soccer.Database.Teams;
using Soccer.EventProcessors.Leagues.Filters;

namespace Soccer.EventProcessors.Teams
{
    public class FetchTeamResultsConsumer : IConsumer<ITeamResultsFetchedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly IMajorLeagueFilter<Match, bool> matchFilter;

        public FetchTeamResultsConsumer(IDynamicRepository dynamicRepository, IMajorLeagueFilter<Match, bool> matchFilter)
        {
            this.dynamicRepository = dynamicRepository;
            this.matchFilter = matchFilter;
        }

        public async Task Consume(ConsumeContext<ITeamResultsFetchedMessage> context)
        {
            var message = context?.Message;

            if (message?.TeamResult != null)
            {
                var isBelongToMajorLeague = await matchFilter.Filter(message.TeamResult);

                if (isBelongToMajorLeague)
                {
                    var homeTeamId = message.TeamResult.Teams?.FirstOrDefault(t => t.IsHome)?.Id;
                    var awayTeamId = message.TeamResult.Teams?.FirstOrDefault(t => !t.IsHome)?.Id;

                    await dynamicRepository.ExecuteAsync(new InsertOrUpdateHeadToHeadCommand(
                        homeTeamId,
                        awayTeamId,
                        message.TeamResult,
                        message.Language));
                }
            }
        }
    }
}
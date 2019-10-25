using System.Threading.Tasks;
using Fanex.Data.Repository;
using MassTransit;
using Soccer.Core.Matches.Models;
using Soccer.Core.Teams.QueueMessages;
using Soccer.Database.Teams;
using Soccer.EventProcessors.Leagues.Filters;

namespace Soccer.EventProcessors.Teams
{
    public class FetchHeadToHeadConsumer : IConsumer<IHeadToHeadFetchedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly IMajorLeagueFilter<Match, bool> matchFilter;

        public FetchHeadToHeadConsumer(IDynamicRepository dynamicRepository, IMajorLeagueFilter<Match, bool> matchFilter)
        {
            this.dynamicRepository = dynamicRepository;
            this.matchFilter = matchFilter;
        }

        public async Task Consume(ConsumeContext<IHeadToHeadFetchedMessage> context)
        {
            var message = context?.Message;

            if (message != null)
            {
                var isBelongToMajorLeague = await matchFilter.Filter(message.HeadToHeadMatch);

                if (isBelongToMajorLeague)
                {
                    await dynamicRepository.ExecuteAsync(new InsertOrUpdateHeadToHeadCommand(
                        message.HomeTeamId,
                        message.AwayTeamId,
                        message.HeadToHeadMatch,
                        message.Language));
                }
            }
        }
    }
}
using System.Threading.Tasks;
using Fanex.Data.Repository;
using MassTransit;
using Soccer.Core.Leagues.Extensions;
using Soccer.Core.Matches.Models;
using Soccer.Core.Teams.QueueMessages;
using Soccer.Database.Teams;
using Soccer.EventProcessors.Leagues.Filters;
using Soccer.EventProcessors.Leagues.Services;

namespace Soccer.EventProcessors.Teams
{
    public class FetchHeadToHeadConsumer : IConsumer<IHeadToHeadFetchedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly IMajorLeagueFilter<Match, bool> matchFilter;
        private readonly ILeagueService leagueService;

        public FetchHeadToHeadConsumer(
            IDynamicRepository dynamicRepository,
            IMajorLeagueFilter<Match, bool> matchFilter,
            ILeagueService leagueService)
        {
            this.dynamicRepository = dynamicRepository;
            this.matchFilter = matchFilter;
            this.leagueService = leagueService;
        }

        public async Task Consume(ConsumeContext<IHeadToHeadFetchedMessage> context)
        {
            var message = context?.Message;

            if (message != null)
            {
                var isBelongToMajorLeague = await matchFilter.Filter(message.HeadToHeadMatch);

                if (isBelongToMajorLeague)
                {
                    var majorLeagues = await leagueService.GetMajorLeagues();
                    message.HeadToHeadMatch.League.UpdateMajorLeagueInfo(majorLeagues);

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
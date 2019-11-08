using System.Linq;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using MassTransit;
using Soccer.Core.Leagues.Extensions;
using Soccer.Core.Teams.QueueMessages;
using Soccer.Database.Teams;
using Soccer.EventProcessors.Leagues.Services;

namespace Soccer.EventProcessors.Teams
{
    public class FetchTeamResultsConsumer : IConsumer<ITeamResultsFetchedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly ILeagueService leagueService;

        public FetchTeamResultsConsumer(
            IDynamicRepository dynamicRepository,
            ILeagueService leagueService)
        {
            this.dynamicRepository = dynamicRepository;
            this.leagueService = leagueService;
        }

        public async Task Consume(ConsumeContext<ITeamResultsFetchedMessage> context)
        {
            var message = context?.Message;

            if (message?.TeamResult != null)
            {
                var majorLeagues = await leagueService.GetMajorLeagues();
                message.TeamResult.League.UpdateMajorLeagueInfo(majorLeagues);

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
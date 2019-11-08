using System.Threading.Tasks;
using Fanex.Data.Repository;
using MassTransit;
using Soccer.Core.Leagues.Extensions;
using Soccer.Core.Teams.QueueMessages;
using Soccer.Database.Teams;
using Soccer.EventProcessors.Leagues.Services;

namespace Soccer.EventProcessors.Teams
{
    public class FetchHeadToHeadConsumer : IConsumer<IHeadToHeadFetchedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly ILeagueService leagueService;

        public FetchHeadToHeadConsumer(
            IDynamicRepository dynamicRepository,
            ILeagueService leagueService)
        {
            this.dynamicRepository = dynamicRepository;
            this.leagueService = leagueService;
        }

        public async Task Consume(ConsumeContext<IHeadToHeadFetchedMessage> context)
        {
            var message = context?.Message;

            if (message != null)
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
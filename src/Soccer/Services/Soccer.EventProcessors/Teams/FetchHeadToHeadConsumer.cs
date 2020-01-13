using System;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using MassTransit;
using Soccer.Core.Leagues.Extensions;
using Soccer.Core.Teams.QueueMessages;
using Soccer.Database.Teams;
using Soccer.EventProcessors.Leagues.Services;
using Soccer.EventProcessors.Shared.Configurations;

namespace Soccer.EventProcessors.Teams
{
    public class FetchHeadToHeadConsumer : IConsumer<IHeadToHeadFetchedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly ILeagueService leagueService;
        private readonly IAppSettings appSettings;

        public FetchHeadToHeadConsumer(
            IDynamicRepository dynamicRepository,
            ILeagueService leagueService,
            IAppSettings appSettings)
        {
            this.dynamicRepository = dynamicRepository;
            this.leagueService = leagueService;
            this.appSettings = appSettings;
        }

        public async Task Consume(ConsumeContext<IHeadToHeadFetchedMessage> context)
        {
            var message = context?.Message;

            if (message?.Match != null && message.Match.EventDate >= DateTime.Now.AddYears(-appSettings.HeadToHeadIntervalInYears))
            {
                var majorLeagues = await leagueService.GetMajorLeagues();
                message.Match.League.UpdateMajorLeagueInfo(majorLeagues);

                var homeTeamId = message.Match.Teams?.FirstOrDefault(t => t.IsHome)?.Id;
                var awayTeamId = message.Match.Teams?.FirstOrDefault(t => !t.IsHome)?.Id;

                await dynamicRepository.ExecuteAsync(new InsertOrUpdateHeadToHeadCommand(
                    homeTeamId,
                    awayTeamId,
                    message.Match,
                    message.Language));
            }
        }
    }
}
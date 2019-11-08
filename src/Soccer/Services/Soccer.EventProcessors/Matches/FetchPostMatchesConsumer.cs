using System.Linq;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using MassTransit;
using Soccer.Core.Leagues.Extensions;
using Soccer.Core.Matches.Events;
using Soccer.Database.Matches.Commands;
using Soccer.EventProcessors.Leagues.Services;

namespace Soccer.EventProcessors.Matches
{
    public class FetchPostMatchesConsumer : IConsumer<IPostMatchFetchedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly ILeagueService leagueService;

        public FetchPostMatchesConsumer(
            IDynamicRepository dynamicRepository,
            ILeagueService leagueService)
        {
            this.dynamicRepository = dynamicRepository;
            this.leagueService = leagueService;
        }

        public async Task Consume(ConsumeContext<IPostMatchFetchedMessage> context)
        {
            var message = context.Message;
            var majorLeagues = await leagueService.GetMajorLeagues();
            var updatedMatches = message.Matches
                .Select(match =>
                {
                    match.League.UpdateMajorLeagueInfo(majorLeagues);

                    return match;
                });

            var command = new InsertOrUpdatePostMatchesCommand(updatedMatches, message.Language);

            await dynamicRepository.ExecuteAsync(command);
        }
    }
}
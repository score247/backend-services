using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using MassTransit;
using Score247.Shared.Enumerations;
using Soccer.Core.Leagues.Extensions;
using Soccer.Core.Matches.Events;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.Core.Teams.QueueMessages;
using Soccer.Database.Matches.Commands;
using Soccer.EventProcessors.Leagues.Services;

namespace Soccer.EventProcessors.Matches
{
    public class FetchPreMatchesConsumer : IConsumer<IPreMatchesFetchedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly ILeagueService leagueService;
        private readonly IBus messageBus;

        public FetchPreMatchesConsumer(
            IDynamicRepository dynamicRepository,
            ILeagueService leagueService,
            IBus messageBus)
        {
            this.dynamicRepository = dynamicRepository;
            this.leagueService = leagueService;
            this.messageBus = messageBus;
        }

        public async Task Consume(ConsumeContext<IPreMatchesFetchedMessage> context)
        {
            var message = context.Message;

            if (message == null || !message.Matches.Any())
            {
                return;
            }

            var majorLeagues = await leagueService.GetMajorLeagues();

            var updatedMatches = message.Matches
                .Select(match =>
                {
                    match.League.UpdateMajorLeagueInfo(majorLeagues);

                    return match;
                }).GroupBy(match => match.EventDate.Date);

            foreach (var matchGroup in updatedMatches)
            {
                var matches = matchGroup.ToList();
                var command = new InsertOrUpdateMatchesCommand(matches, message.Language, matchGroup.Key);

                await dynamicRepository.ExecuteAsync(command);

                await PublishHeadToHeadMessages(matches, message.Language);
            }
        }

        private async Task PublishHeadToHeadMessages(IList<Match> matches, string language) 
        {
            foreach (var match in matches)
            {
                await messageBus.Publish<IHeadToHeadFetchedMessage>(
                              new HeadToHeadFetchedMessage(match, Enumeration.FromDisplayName<Language>(language)));
            }
        }
    }
}
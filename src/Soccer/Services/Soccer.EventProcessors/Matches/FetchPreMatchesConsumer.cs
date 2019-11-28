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
using Soccer.Database.Matches.Commands;
using Soccer.Database.Teams;
using Soccer.EventProcessors.Leagues.Services;

namespace Soccer.EventProcessors.Matches
{
    public class FetchPreMatchesConsumer : IConsumer<IPreMatchesFetchedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly ILeagueService leagueService;

        public FetchPreMatchesConsumer(
            IDynamicRepository dynamicRepository,
            ILeagueService leagueService)
        {
            this.dynamicRepository = dynamicRepository;
            this.leagueService = leagueService;
        }

        public async Task Consume(ConsumeContext<IPreMatchesFetchedMessage> context)
        {
            var message = context.Message;
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

                await InsertHeadToHeads(matches, message.Language);
            }
        }

        private async Task InsertHeadToHeads(IList<Match> matches, string language) 
        {
            foreach (var match in matches)
            {
                if (match.Teams?.Any() == false)
                {
                    continue;
                }

                var homeTeamId = match.Teams.FirstOrDefault(t => t.IsHome)?.Id;
                var awayTeamId = match.Teams.FirstOrDefault(t => !t.IsHome)?.Id;

                await dynamicRepository.ExecuteAsync(new InsertOrUpdateHeadToHeadCommand(
                    homeTeamId,
                    awayTeamId,
                    match,
                    Enumeration.FromDisplayName<Language>(language)));
            }            
        }
    }
}
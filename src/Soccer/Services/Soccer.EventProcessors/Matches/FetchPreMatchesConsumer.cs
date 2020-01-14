using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using MassTransit;
using Score247.Shared.Enumerations;
using Soccer.Core.Leagues.Extensions;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Leagues.QueueMessages;
using Soccer.Core.Matches.Models;
using Soccer.Core.Matches.QueueMessages;
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

            if (message?.Matches.Any() != true)
            {
                return;
            }

            var majorLeagues = await leagueService.GetMajorLeagues();

            var updatedMatches = message.Matches
                .Select(match =>
                {
                    match.League.UpdateMajorLeagueInfo(majorLeagues);
                    var leagueGroupStage = BuildLeagueGroupStage(match);
                    match.UpdateLeagueGroupStage(leagueGroupStage);

                    return match;
                }).ToList();

            var leagueGroupStages = updatedMatches.Select(match => match.LeagueGroupStage).Distinct();
            await PublicInsertLeagueGroupStages(leagueGroupStages, message.Language);

            var matchesByDate = updatedMatches.GroupBy(match => match.EventDate.Date);

            await Task.WhenAll(
                PublishHeadToHeadMessages(updatedMatches, message.Language),
                InsertMatchesToDatabase(matchesByDate, message));
        }

        private async Task InsertMatchesToDatabase(IEnumerable<IGrouping<DateTime, Match>> matchesByDate, IPreMatchesFetchedMessage message)
        {
            foreach (var matchGroup in matchesByDate)
            {
                var matches = matchGroup.ToList();
                var command = new InsertOrUpdateMatchesCommand(matches, message.Language, matchGroup.Key);

                await dynamicRepository.ExecuteAsync(command);
            }
        }

        private static LeagueGroupStage BuildLeagueGroupStage(Match match)
        {
            var hasStanding = string.IsNullOrEmpty(match.GroupName)
                    || (match.LeagueRound.Type.DisplayName == LeagueRoundType.Group && match.LeagueRound.Group != null);

            return new LeagueGroupStage(
                match.League.Id,
                match.LeagueSeason.Id,
                match.LeagueGroupName,
                match.GroupName,
                match.LeagueRound,
                hasStanding);
        }

        private async Task PublicInsertLeagueGroupStages(IEnumerable<LeagueGroupStage> leagueGroupStages, string language)
        {
            foreach (var leagueGroupStage in leagueGroupStages)
            {
                await messageBus.Publish<ILeagueGroupFetchedMessage>(
                              new LeagueGroupFetchedMessage(leagueGroupStage, Enumeration.FromDisplayName<Language>(language)));
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
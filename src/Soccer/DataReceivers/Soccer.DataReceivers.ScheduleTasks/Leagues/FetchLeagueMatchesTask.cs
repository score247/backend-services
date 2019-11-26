﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using MassTransit;
using Score247.Shared.Enumerations;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Leagues.QueueMessages;
using Soccer.Core.Matches.Events;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.DataProviders.Leagues;
using Soccer.DataReceivers.ScheduleTasks.Matches;
using Soccer.DataReceivers.ScheduleTasks.Shared.Configurations;
using Soccer.DataReceivers.ScheduleTasks.Teams;

namespace Soccer.DataReceivers.ScheduleTasks.Leagues
{
    public interface IFetchLeagueMatchesTask
    {
        [AutomaticRetry(Attempts = 1)]
        [Queue("low")]
        Task FetchLeagueMatches();

        [AutomaticRetry(Attempts = 1)]
        [Queue("low")]
        Task FetchMatchesForLeague(IList<LeagueSeasonProcessedInfo> leagueSeasons);
    }
    public class FetchLeagueMatchesTask : IFetchLeagueMatchesTask
    {
        private const int BactchOfLeagueSize = 5;
        private const int BactchOfMatchSize = 15;
        private readonly TimeSpan TeamResultsDelayTimespan;

        private readonly IAppSettings appSettings;
        private readonly IBus messageBus;

        private readonly ILeagueScheduleService leagueScheduleService;
        private readonly ILeagueSeasonService leagueSeasonService;
        private readonly IBackgroundJobClient jobClient;

        public FetchLeagueMatchesTask(
            IBus messageBus,
            IAppSettings appSettings,
            ILeagueScheduleService leagueScheduleService,
            ILeagueSeasonService leagueSeasonService,
            IBackgroundJobClient jobClient)
        {
            this.appSettings = appSettings;
            this.messageBus = messageBus;
            this.leagueScheduleService = leagueScheduleService;
            this.leagueSeasonService = leagueSeasonService;
            this.jobClient = jobClient;

            TeamResultsDelayTimespan = TimeSpan.FromMinutes(appSettings.ScheduleTasksSettings.FetchTeamResultsDelayedMinutes);
        }

        public async Task FetchLeagueMatches()
        {
            var unprocessedLeagueSeason = await leagueSeasonService.GetUnprocessedLeagueSeason();

            if (unprocessedLeagueSeason?.Any() == false)
            {
                return;
            }

            for (var i = 0; i * BactchOfLeagueSize < unprocessedLeagueSeason.Count(); i++)
            {
                var batchOfLeague = unprocessedLeagueSeason.Skip(i * BactchOfLeagueSize).Take(BactchOfLeagueSize).ToList();

                jobClient.Enqueue<IFetchLeagueMatchesTask>(t => t.FetchMatchesForLeague(batchOfLeague));
            }
        }

        public async Task FetchMatchesForLeague(IList<LeagueSeasonProcessedInfo> leagueSeasons)
        {
            foreach (var season in leagueSeasons)
            {
                foreach (var language in Enumeration.GetAll<Language>())
                {
                    var matches = await leagueScheduleService.GetLeagueMatches(season.Region, season.LeagueId, language);
                    await PublishPreMatchesMessage(language, matches);

                    ScheduleTimelineAndLineUpsTasks(matches.Where(match => match.MatchResult.EventStatus.IsClosed()), language);
                    ScheduleTeamResultsTasks(language, matches);
                }
            }

            await messageBus.Publish<ILeagueMatchesFetchedMessage>(
                new LeagueMatchesFetchedMessage(leagueSeasons));
        }

        private async Task PublishPreMatchesMessage(Language language, IEnumerable<Match> matches)
        {
            var batchSize = appSettings.ScheduleTasksSettings.QueueBatchSize;

            for (var i = 0; i * batchSize < matches.Count(); i++)
            {
                var batchOfMatches = matches.Skip(i * batchSize).Take(batchSize).ToList();

                await messageBus.Publish<IPreMatchesFetchedMessage>(
                    new PreMatchesFetchedMessage(batchOfMatches, language.DisplayName));
            }
        }

        private void ScheduleTimelineAndLineUpsTasks(IEnumerable<Match> closedMatches, Language language)
        {
            for (var i = 0; i * BactchOfMatchSize < closedMatches.Count(); i++)
            {
                var batchOfMatches = closedMatches.Skip(i * BactchOfMatchSize).Take(BactchOfMatchSize).ToList();

                var timelineDelayTimespan = TimeSpan.FromMinutes(appSettings.ScheduleTasksSettings.FetchTimelineDelayedMinutes + i);
                var lineupsDelayTimespan = TimeSpan.FromMinutes(appSettings.ScheduleTasksSettings.FetchLineupsDelayedMinutes + i);

                jobClient.Schedule<IFetchTimelineTask>(t => t.FetchTimelinesForClosedMatch(batchOfMatches, language), timelineDelayTimespan);
                jobClient.Schedule<IFetchMatchLineupsTask>(t => t.FetchMatchLineupsForCLosedMatch(batchOfMatches, language), lineupsDelayTimespan);
            }
        }

        private void ScheduleTeamResultsTasks(Language language, IEnumerable<Match> matches)
        {
            var teams = matches
                .SelectMany(match => match.Teams)
                .GroupBy(team => team.Id)
                .Select(grp => grp.FirstOrDefault());

            if (teams?.Any() == true)
            {
                jobClient.Schedule<IFetchHeadToHeadsTask>(task => task.FetchTeamResults(teams, language), TeamResultsDelayTimespan);
            }
        }
    }
}

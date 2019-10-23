using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Hangfire;
using MassTransit;
using Soccer.Core.Shared.Enumerations;
using Soccer.DataProviders.Matches.Services;
using Soccer.DataProviders.Teams.Services;
using Soccer.DataReceivers.ScheduleTasks.Shared.Configurations;

namespace Soccer.DataReceivers.ScheduleTasks.Teams
{
    public interface IFetchTeamHeadToHeadTask
    {
        [Queue("medium")]
        Task FetchTeamHeadToHead(string homeTeamId, string awayTeamId, Language language);
    }

    public class FetchTeamHeadToHeadTask : IFetchTeamHeadToHeadTask
    {
        private readonly ITeamHeadToHeadService teamHeadToHeadService;
        private readonly IBus messageBus;

        public FetchTeamHeadToHeadTask(
            ITeamHeadToHeadService teamHeadToHeadService,
            IBus messageBus)
        {
            this.teamHeadToHeadService = teamHeadToHeadService;
            this.messageBus = messageBus;
        }

        public async Task FetchTeamHeadToHead(string homeTeamId, string awayTeamId, Language language)
        {
            var teamHeadToHeads = await teamHeadToHeadService.GetTeamHeadToHeads(homeTeamId, awayTeamId, language);
        }
    }
}
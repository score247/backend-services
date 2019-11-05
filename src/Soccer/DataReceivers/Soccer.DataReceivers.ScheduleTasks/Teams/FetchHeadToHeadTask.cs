using System.Threading.Tasks;
using Hangfire;
using MassTransit;
using Microsoft.EntityFrameworkCore.Internal;
using Soccer.Core.Shared.Enumerations;
using Soccer.Core.Teams.QueueMessages;
using Soccer.DataProviders.Teams.Services;

namespace Soccer.DataReceivers.ScheduleTasks.Teams
{
    public interface IFetchHeadToHeadsTask
    {
        [Queue("medium")]
        Task FetchHeadToHeads(string homeTeamId, string awayTeamId, Language language);

        [Queue("medium")]
        Task FetchTeamResults(string teamId, Language language);
    }

    public class FetchHeadToHeadsTask : IFetchHeadToHeadsTask
    {
        private readonly IHeadToHeadService headToHeadService;
        private readonly IBus messageBus;

        public FetchHeadToHeadsTask(
            IHeadToHeadService headToHeadService,
            IBus messageBus)
        {
            this.headToHeadService = headToHeadService;
            this.messageBus = messageBus;
        }

        public async Task FetchHeadToHeads(string homeTeamId, string awayTeamId, Language language)
        {
            if (string.IsNullOrEmpty(homeTeamId) || string.IsNullOrEmpty(awayTeamId))
            {
                return;
            }

            var headToHeadMatches = await headToHeadService.GetTeamHeadToHeads(homeTeamId, awayTeamId, language);

            if (headToHeadMatches?.Any() == true)
            {
                foreach (var headToHeadMatch in headToHeadMatches)
                {
                    await messageBus.Publish<IHeadToHeadFetchedMessage>(
                          new HeadToHeadFetchedMessage(homeTeamId, awayTeamId, headToHeadMatch, language));
                }
            }
        }

        public async Task FetchTeamResults(string teamId, Language language)
        {
            if (string.IsNullOrEmpty(teamId))
            {
                return;
            }

            var teamResults = await headToHeadService.GetTeamResults(teamId, language);

            if (teamResults?.Any() == true)
            {
                foreach (var teamResult in teamResults)
                {
                    await messageBus.Publish<ITeamResultsFetchedMessage>(
                        new TeamResultsFetchedMessage(teamResult, language));
                }
            }
        }
    }
}
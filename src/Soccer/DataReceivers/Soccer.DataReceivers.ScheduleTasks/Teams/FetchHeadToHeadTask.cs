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
            var headToHeadMatches = await headToHeadService.GetTeamHeadToHeads(homeTeamId, awayTeamId, language);

            if (headToHeadMatches?.Any() == true)
            {
                Parallel.ForEach(headToHeadMatches, match => messageBus.Publish<IHeadToHeadFetchedMessage>(
                    new HeadToHeadFetchedMessage(homeTeamId, awayTeamId, match, language)));
            }
        }
    }
}
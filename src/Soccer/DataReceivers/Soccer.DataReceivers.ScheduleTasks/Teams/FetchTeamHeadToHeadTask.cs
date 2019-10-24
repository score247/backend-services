using System.Threading.Tasks;
using Hangfire;
using MassTransit;
using Microsoft.EntityFrameworkCore.Internal;
using Soccer.Core.Shared.Enumerations;
using Soccer.Core.Teams.QueueMessages;
using Soccer.DataProviders.Teams.Services;

namespace Soccer.DataReceivers.ScheduleTasks.Teams
{
    public interface IFetchTeamHeadToHeadTask
    {
        [Queue("medium")]
        Task FetchTeamHeadToHead(string homeTeamId, string awayTeamId, Language language);
    }

    public class FetchTeamHeadToHeadTask : IFetchTeamHeadToHeadTask
    {
        private readonly IHeadToHeadService headToHeadService;
        private readonly IBus messageBus;

        public FetchTeamHeadToHeadTask(
            IHeadToHeadService headToHeadService,
            IBus messageBus)
        {
            this.headToHeadService = headToHeadService;
            this.messageBus = messageBus;
        }

        public async Task FetchTeamHeadToHead(string homeTeamId, string awayTeamId, Language language)
        {
            var headToHeads = await headToHeadService.GetTeamHeadToHeads(homeTeamId, awayTeamId, language);

            if (headToHeads?.Any() == true)
            {
                await messageBus.Publish<IHeadToHeadFetchedMessage>(
                     new HeadToHeadFetchedMessage(homeTeamId, awayTeamId, headToHeads));
            }
        }
    }
}
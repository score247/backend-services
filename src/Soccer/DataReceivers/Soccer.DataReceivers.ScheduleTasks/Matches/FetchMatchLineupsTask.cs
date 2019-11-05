using System;
using System.Threading.Tasks;
using Hangfire;
using MassTransit;
using Score247.Shared.Enumerations;
using Soccer.Core.Matches.QueueMessages;
using Soccer.Core.Shared.Enumerations;
using Soccer.DataProviders.Matches.Services;

namespace Soccer.DataReceivers.ScheduleTasks.Matches
{
    public interface IFetchMatchLineupsTask
    {
        [Queue("medium")]
        Task FetchMatchLineups(string matchId, string regionName);

        [Queue("medium")]
        Task FetchMatchLineups(string matchId, string region, Language language);

        [Queue("medium")]
        Task FetchMatchLineups();
    }

    public class FetchMatchLineupsTask : IFetchMatchLineupsTask
    {
        private readonly IMatchService matchService;
        private readonly IBus messageBus;

        public FetchMatchLineupsTask(
            IMatchService matchService,
            IBus messageBus)
        {
            this.matchService = matchService;
            this.messageBus = messageBus;
        }

        public async Task FetchMatchLineups(string matchId, string regionName)
        {
            foreach (var language in Enumeration.GetAll<Language>())
            {
                await FetchMatchLineups(matchId, regionName, language);
            }
        }

        public async Task FetchMatchLineups(string matchId, string region, Language language)
        {
            if (!string.IsNullOrWhiteSpace(matchId))
            {
                var matchLineups = await matchService.GetLineups(matchId, region, language);

                if (matchLineups != null
                    && !string.IsNullOrWhiteSpace(matchLineups.Id))
                {
                    await messageBus.Publish<IMatchLineupsMessage>(new MatchLineupsMessage(matchLineups, language));
                }
            }
        }

        public async Task FetchMatchLineups()
        {
            foreach (var language in Enumeration.GetAll<Language>())
            {
                var todayMatches = await matchService.GetPreMatches(DateTime.Now.Date, language);

                foreach (var match in todayMatches)
                {
                    await FetchMatchLineups(match.Id, match.Region, language);
                }
            }
        }
    }
}
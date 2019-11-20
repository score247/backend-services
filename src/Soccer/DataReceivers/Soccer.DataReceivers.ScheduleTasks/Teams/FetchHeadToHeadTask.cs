using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using MassTransit;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.Core.Teams.QueueMessages;
using Soccer.DataProviders.Teams.Services;

namespace Soccer.DataReceivers.ScheduleTasks.Teams
{
    public interface IFetchHeadToHeadsTask
    {
        [AutomaticRetry(Attempts = 1)]
        [Queue("medium")]
        void FetchHeadToHeads(Language language, IEnumerable<Match> matches);

        [AutomaticRetry(Attempts = 1)]
        [Queue("medium")]
        void FetchTeamResults(Language language, IEnumerable<Match> matches);

        [AutomaticRetry(Attempts = 1)]
        [Queue("medium")]
        Task PublishHeadToHeads(Language language, IEnumerable<Match> matches);

        [AutomaticRetry(Attempts = 1)]
        [Queue("medium")]
        Task FetchHeadToHeads(string homeTeamId, string awayTeamId, Language language);

        [AutomaticRetry(Attempts = 1)]
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

        public void FetchHeadToHeads(Language language, IEnumerable<Match> matches)
        {
            foreach (var match in matches)
            {
                (var homeTeamId, var awayTeamId) = GenerateTeamId(match);

                BackgroundJob.Enqueue<IFetchHeadToHeadsTask>(t =>
                    t.FetchHeadToHeads(homeTeamId, awayTeamId, language));
            }
        }

        public void FetchTeamResults(Language language, IEnumerable<Match> matches)
        {
            foreach (var match in matches)
            {
                (var homeTeamId, var awayTeamId) = GenerateTeamId(match);

                BackgroundJob.Enqueue<IFetchHeadToHeadsTask>(t =>
                    t.FetchTeamResults(homeTeamId, language));

                BackgroundJob.Enqueue<IFetchHeadToHeadsTask>(t =>
                    t.FetchTeamResults(awayTeamId, language));
            }
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
                          new HeadToHeadFetchedMessage(headToHeadMatch, language));
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
                    await messageBus.Publish<IHeadToHeadFetchedMessage>(
                        new HeadToHeadFetchedMessage(teamResult, language));
                }
            }
        }

        public async Task PublishHeadToHeads(Language language, IEnumerable<Match> matches)
        {
            foreach (var match in matches)
            {
                await messageBus.Publish<IHeadToHeadFetchedMessage>(
                        new HeadToHeadFetchedMessage(match, language));
            }
        }

        private static (string, string) GenerateTeamId(Match match)
            => (match.Teams.FirstOrDefault(t => t.IsHome)?.Id,
                match.Teams.FirstOrDefault(t => !t.IsHome)?.Id);
    }
}
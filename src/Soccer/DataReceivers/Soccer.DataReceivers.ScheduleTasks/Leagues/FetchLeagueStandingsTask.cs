using System;
using System.Threading.Tasks;
using Hangfire;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.DataReceivers.ScheduleTasks.Leagues
{
    public interface IFetchLeagueStandingsTask
    {
        [AutomaticRetry(Attempts = 1)]
        [Queue("medium")]
        Task FetchLeagueStandings(string leagueId, string region, Language language);
    }

    public class FetchLeagueStandingsTask : IFetchLeagueStandingsTask
    {
        public Task FetchLeagueStandings(string leagueId, string region, Language language)
        {
            throw new NotImplementedException();
        }
    }
}
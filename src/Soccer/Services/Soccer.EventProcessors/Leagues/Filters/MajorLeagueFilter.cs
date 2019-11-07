using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Soccer.Core.Matches.Models;
using Soccer.EventProcessors.Leagues.Services;

namespace Soccer.EventProcessors.Leagues.Filters
{
    public interface IMajorLeagueFilter<in T, TResult>
    {
        Task<TResult> Filter(T data);
    }

    public class MajorLeagueFilter :
        IMajorLeagueFilter<IEnumerable<Match>, IEnumerable<Match>>,
        IMajorLeagueFilter<MatchEvent, bool>,
        IMajorLeagueFilter<Match, bool>,
        IMajorLeagueFilter<string, bool>
    {
        private readonly ILeagueService leagueService;

        public MajorLeagueFilter(ILeagueService leagueService)
        {
            this.leagueService = leagueService;
        }

        public async Task<IEnumerable<Match>> Filter(IEnumerable<Match> data)
        {
            var majorLeagues = await leagueService.GetMajorLeagues();

            return data
                .Where(match => majorLeagues?.Any(league => league.Id == match.League.Id) == true);
        }

        public async Task<bool> Filter(MatchEvent data)
        {
            if (data == null)
            {
                return false;
            }

            return await IsBelongMajorLeagues(data.LeagueId);
        }

        public async Task<bool> Filter(Match data)
        {
            if (data == null)
            {
                return false;
            }

            return await IsBelongMajorLeagues(data.League.Id);
        }

        public Task<bool> Filter(string data)
            => IsBelongMajorLeagues(data);

        private async Task<bool> IsBelongMajorLeagues(string leagueId)
        {
            var majorLeagues = await leagueService.GetMajorLeagues();

            return majorLeagues?.Any(league => league.Id == leagueId) == true;
        }
    }
}
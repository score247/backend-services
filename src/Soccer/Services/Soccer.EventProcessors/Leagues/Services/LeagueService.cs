using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using Soccer.Cache.Leagues;
using Soccer.Core.Leagues.Models;
using Soccer.Database.Leagues.Criteria;

namespace Soccer.EventProcessors.Leagues.Services
{
    public interface ILeagueService
    {
        Task<IEnumerable<League>> GetMajorLeagues();
    }

    public class LeagueService : ILeagueService
    {
        private readonly ILeagueCache leagueCache;
        private readonly IDynamicRepository dynamicRepository;

        public LeagueService(ILeagueCache leagueCache, IDynamicRepository dynamicRepository)
        {
            this.leagueCache = leagueCache;
            this.dynamicRepository = dynamicRepository;
        }

        public async Task<IEnumerable<League>> GetMajorLeagues()
        {
            var cachedMajorLeagues = (await leagueCache.GetMajorLeagues())?.ToList();

            if (cachedMajorLeagues?.Any() == false)
            {
                return cachedMajorLeagues;
            }

            var majorLeagues = (await dynamicRepository.FetchAsync<League>(new GetActiveLeaguesCriteria())).ToList();
            await leagueCache.SetMajorLeagues(majorLeagues);

            return majorLeagues;
        }
    }
}
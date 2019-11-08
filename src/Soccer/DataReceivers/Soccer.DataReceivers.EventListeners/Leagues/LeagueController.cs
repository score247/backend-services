using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Soccer.Cache.Leagues;

namespace Soccer.DataReceivers.EventListeners.Leagues
{
    [Route("eventlistener/soccer/{language}/leagues")]
    [ApiController]
    public class LeagueController : ControllerBase
    {
        private readonly ILeagueCache leagueCache;

        public LeagueController(ILeagueCache leagueCache)
        {
            this.leagueCache = leagueCache;
        }

        [HttpPost]
        [Route("major/cleancache")]
        public Task CleanMajorLeagueCache()
            => leagueCache.ClearMajorLeaguesCache();
    }
}
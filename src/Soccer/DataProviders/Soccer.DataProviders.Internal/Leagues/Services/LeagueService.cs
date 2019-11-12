﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Logging;
using Refit;
using Soccer.Cache.Leagues;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.DataProviders.Leagues;

namespace Soccer.DataProviders.Internal.Leagues.Services
{
    public interface IInternalLeagueApi
    {
        [Get("/soccer/{language}/leagues/major")]
        Task<IEnumerable<League>> GetMajorLeagues(string language);
    }

    public class InternalLeagueService : ILeagueService
    {
        private readonly IInternalLeagueApi leagueApi;
        private readonly ILeagueCache leagueCache;
        private readonly ILogger logger;

        public InternalLeagueService(IInternalLeagueApi leagueApi, ILeagueCache leagueCache, ILogger logger)
        {
            this.leagueApi = leagueApi;
            this.leagueCache = leagueCache;
            this.logger = logger;
        }

        public async Task<IEnumerable<League>> GetLeagues(Language language)
        {
            try
            {
                var cachedMajorLeagues = (await leagueCache.GetMajorLeagues(language.DisplayName))?.ToList();

                if (cachedMajorLeagues?.Any() == false)
                {
                    return cachedMajorLeagues;
                }

                var majorLeagues = await leagueApi.GetMajorLeagues(language.DisplayName);

                await leagueCache.SetMajorLeagues(majorLeagues);

                return majorLeagues;
            }
            catch (Exception ex)
            {
                await logger.ErrorAsync(ex.Message, ex);

                return Enumerable.Empty<League>();
            }
        }
    }
}
namespace Soccer.API.Matches
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using Soccer.API._Shared;
    using Soccer.API.Matches.Models;
    using Soccer.API.Shared.Configurations;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Shared.Enumerations;
    using Soccer.Database.Matches.Criteria;

    public interface IMatchQueryService
    {
        Task<IEnumerable<MatchSummary>> GetByDateRange(DateTime from, DateTime to, Language language);

        Task<MatchInfo> GetMatchInfo(string id, Language language);

        Task<IEnumerable<MatchSummary>> GetLive(Language language);

        Task<int> GetLiveMatchCount(Language language);
    }

    public class MatchQueryService : IMatchQueryService
    {
        private const string MatchInfoCacheKey = "MatchQuery_MatchInfoCacheKey";
        private const string MatchListCacheKey = "MatchQuery_MatchListCacheKey";
        private const string FormatDate = "yyyyMMdd-hhmmss";
        private readonly IDynamicRepository dynamicRepository;
        private readonly ICacheManager cacheManager;
        private readonly IAppSettings appSettings;
        private readonly Func<DateTimeOffset> dateTimeNowFunc;

        public MatchQueryService(
            IDynamicRepository dynamicRepository,
            ICacheManager cacheManager,
            IAppSettings appSettings,
            Func<DateTimeOffset> dateTimeNowFunc)
        {
            this.dynamicRepository = dynamicRepository;
            this.cacheManager = cacheManager;
            this.appSettings = appSettings;
            this.dateTimeNowFunc = dateTimeNowFunc;
        }

        public async Task<IEnumerable<MatchSummary>> GetLive(Language language)
        {
            var liveMatches = await dynamicRepository.FetchAsync<Match>(new GetLiveMatchesCriteria(language, dateTimeNowFunc().AddDays(-1).DateTime));

            return liveMatches
                .Where(match => IsMatchRunning(match))
                .Select(m => new MatchSummary(m));
        }

        public async Task<int> GetLiveMatchCount(Language language)
            => (await GetLive(language)).Count();

        public async Task<IEnumerable<MatchSummary>> GetByDateRange(DateTime from, DateTime to, Language language)
        {
            var cachedMatches = await GetOrSetAsync(
                MatchListCacheKey,
                from,
                to,
                async () =>
                {
                    var matches = await dynamicRepository.FetchAsync<Match>(new GetMatchesByDateRangeCriteria(from, to, language));

                    return matches.Select(m => new MatchSummary(m));
                });

            return cachedMatches;
        }

        public async Task<MatchInfo> GetMatchInfo(string id, Language language)
        {
            var cacheMatch = await cacheManager.GetAsync<MatchInfo>(BuildMatchInfoCacheKey(id));

            if (cacheMatch == null)
            {
                return await GetAndCacheMatchInfo(id, language);
            }

            return cacheMatch;
        }

        private async Task<MatchInfo> GetAndCacheMatchInfo(string id, Language language)
        {
            var match = await dynamicRepository.GetAsync<Match>(new GetMatchByIdCriteria(id, language));

            if (match != null)
            {
                return await cacheManager.GetOrSetAsync(
                    BuildMatchInfoCacheKey(id),
                    async () =>
                    {
                        var timelineEvents = await dynamicRepository.FetchAsync<TimelineEvent>(new GetTimelineEventsCriteria(id));

                        var matchInfo = new MatchInfo(new MatchSummary(match), timelineEvents, match.Venue, match.Referee, match.Attendance);

                        return matchInfo;
                    },
                    BuildCacheOptions(match.EventDate.DateTime));
            }

            ////TODO: Should fix here
            return null;
        }

        private static string BuildMatchInfoCacheKey(string matchId)
            => $"{MatchInfoCacheKey}_{matchId}";

        private async Task<T> GetOrSetAsync<T>(string key, DateTime from, DateTime to, Func<Task<T>> factory)
        {
            var cacheItemOptions = BuildCacheOptions(from);
            var cacheKey = BuildCacheKey(key, from, to);

            return await cacheManager
                .GetOrSetAsync(cacheKey, factory, cacheItemOptions);
        }

        private CacheItemOptions BuildCacheOptions(DateTime date)
        {
            var cacheDuration = ShouldCacheWithShortDuration(date)
                            ? appSettings.MatchShortCacheTimeDuration
                            : appSettings.MatchLongCacheTimeDuration;

            return new CacheItemOptions().SetAbsoluteExpiration(DateTime.Now.AddSeconds(cacheDuration));
        }

        private bool ShouldCacheWithShortDuration(DateTime date)
            => date.ToUniversalTime().Date == dateTimeNowFunc().UtcDateTime.Date
                || date.ToUniversalTime().Date == dateTimeNowFunc().UtcDateTime.Date.AddDays(-1);

        private static string BuildCacheKey(string key, DateTime from, DateTime to)
            => $"{key}_{from.ToString(FormatDate)}_{to.ToString(FormatDate)}";

        private bool IsMatchRunning(Match match)
        {
            if (match.LatestTimeline.Type.IsMatchEnd()
                && match.LatestTimeline.Time.ToUniversalTime().AddMinutes(appSettings.NumOfMinutesToLoadClosedMatch) < dateTimeNowFunc().ToUniversalTime())
            {
                return false;
            }

            return true;
        }
    }
}
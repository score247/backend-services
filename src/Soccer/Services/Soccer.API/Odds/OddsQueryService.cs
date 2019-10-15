namespace Soccer.API.Odds
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using Score247.Shared;
    using Soccer.API.Shared.Configurations;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Odds;
    using Soccer.Core.Odds.Models;
    using Soccer.Core.Shared.Enumerations;
    using Soccer.Database.Matches.Criteria;
    using Soccer.Database.Odds.Criteria;

    public interface IOddsQueryService
    {
        Task<MatchOdds> GetOdds(string matchId, int betTypeId, Language language);

        Task<MatchOddsMovement> GetOddsMovement(string matchId, int betTypeId, string bookmakerId, Language language);
    }

    public class OddsQueryService : IOddsQueryService
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly IAppSettings appSettings;
        private readonly ICacheManager cacheManager;

        public OddsQueryService(
            IDynamicRepository dynamicRepository,
            IAppSettings appSettings,
            ICacheManager cacheManager)
        {
            this.dynamicRepository = dynamicRepository;
            this.appSettings = appSettings;
            this.cacheManager = cacheManager;
        }

        public async Task<MatchOdds> GetOdds(string matchId, int betTypeId, Language language)
            => new MatchOdds(matchId, await GetBookmakerComparisonOdds(matchId, betTypeId));

        private async Task<IEnumerable<BetTypeOdds>> GetBookmakerComparisonOdds(string matchId, int betTypeId)
        {
            var match = await GetMatch(matchId, Language.en_US, false);

            if (match == null)
            {
                return Enumerable.Empty<BetTypeOdds>();
            }

            return await cacheManager.GetOrSetAsync(
                    $"OddsComparisonCacheKey_{matchId}_{betTypeId}",
                    async () => await GetBetTypeOddsComparisons(matchId, betTypeId, match.EventDate),
                    BuildCacheOptions(match.EventDate.DateTime));
        }

        public async Task<IOrderedEnumerable<BetTypeOdds>> GetBetTypeOddsComparisons(string matchId, int betTypeId, DateTimeOffset eventDate)
        {
            var oddsByBookmaker = (await GetOddsData(matchId, betTypeId)).GroupBy(o => o.Bookmaker?.Id);
            var minDate = eventDate.AddDays(-appSettings.NumOfDaysToShowOddsBeforeKickoffDate).Date;

            var betTypeOdssList = oddsByBookmaker
                .Select(group => OddsMovementProcessor.AssignOpeningOddsToFirstOdds(group, minDate))
                .OrderBy(bto => bto.Bookmaker?.Name);

            return betTypeOdssList;
        }

        private Task<IEnumerable<BetTypeOdds>> GetOddsData(string matchId, int betTypeId)
            => dynamicRepository.FetchAsync<BetTypeOdds>(new GetOddsCriteria(matchId, betTypeId));

        public async Task<MatchOddsMovement> GetOddsMovement(
            string matchId,
            int betTypeId,
            string bookmakerId,
            Language language)
        {
            var match = await GetMatch(matchId, language);

            if (match == null)
            {
                return new MatchOddsMovement();
            }

            return await cacheManager.GetOrSetAsync(
                   $"OddsMovementCacheKey_{matchId}_{betTypeId}_{bookmakerId}_{language.Value}",
                   async () => await GetBookmakerOddsMovement(matchId, betTypeId, bookmakerId, match),
                   BuildCacheOptions(match.EventDate.DateTime));
        }

        public async Task<MatchOddsMovement> GetBookmakerOddsMovement(string matchId, int betTypeId, string bookmakerId, Match match)
        {
            var betTypeOddsList = await GetBookmakerOddsListByBetType(matchId, betTypeId, bookmakerId);
            var firstOdds = betTypeOddsList.FirstOrDefault();

            if (firstOdds == null)
            {
                return new MatchOddsMovement();
            }

            var oddsMovements = OddsMovementProcessor.BuildOddsMovements(match, betTypeOddsList, appSettings.NumOfDaysToShowOddsBeforeKickoffDate);

            return new MatchOddsMovement(matchId, firstOdds.Bookmaker, oddsMovements);
        }

        private async Task<List<BetTypeOdds>> GetBookmakerOddsListByBetType(string matchId, int betTypeId, string bookmakerId)
            => (await dynamicRepository
                .FetchAsync<BetTypeOdds>(new GetOddsCriteria(matchId, betTypeId, bookmakerId)))
                .OrderBy(bto => bto.LastUpdatedTime)
                .ToList();

        public async Task<Match> GetMatch(string matchId, Language language, bool isGetTimeline = true)
        {
            var match = await cacheManager.GetOrSetAsync(
                   $"MatchOddsCacheKey_{matchId}_{language.Value}",
                   async () => await dynamicRepository.GetAsync<Match>(new GetMatchByIdCriteria(matchId, language)),
                   new CacheItemOptions().SetAbsoluteExpiration(DateTime.Now.AddSeconds(3600)));

            if (match != null && isGetTimeline)
            {
                await SetMatchTimelines(match);
            }

            return match;
        }

        private async Task SetMatchTimelines(Match match)
        {
            var timelineEvents = await dynamicRepository.FetchAsync<TimelineEvent>(new GetTimelineEventsCriteria(match.Id));

            if (timelineEvents?.Count() > 0)
            {
                match.TimeLines = timelineEvents.OrderBy(t => t.Time);
            }
        }

        private CacheItemOptions BuildCacheOptions(DateTime date)
        {
            var cacheDuration = ShouldCacheWithShortDuration(date)
                            ? appSettings.OddsShortCacheTimeDuration
                            : appSettings.OddsLongCacheTimeDuration;

            return new CacheItemOptions().SetAbsoluteExpiration(DateTime.Now.AddSeconds(cacheDuration));
        }

        private static bool ShouldCacheWithShortDuration(DateTime date)
            => date.ToUniversalTime().Date >= DateTime.UtcNow.Date;
    }
}
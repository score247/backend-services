[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Soccer.API.Tests")]

namespace Soccer.API.Matches
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using Models;
    using Score247.Shared;
    using Shared.Configurations;
    using Soccer.API.Matches.Helpers;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Shared.Enumerations;
    using Soccer.Core.Teams.Models;
    using Soccer.Database.Matches.Criteria;
    using Soccer.Database.Timelines.Criteria;

    public interface IMatchQueryService
    {
        Task<IEnumerable<MatchSummary>> GetByDateRange(DateTimeOffset from, DateTimeOffset to, Language language);

        Task<MatchInfo> GetMatchInfo(string id, Language language, DateTimeOffset eventDate);

        Task<IEnumerable<MatchSummary>> GetLive(Language language);

        Task<int> GetLiveMatchCount(Language language);

        Task<MatchCoverage> GetMatchCoverage(string id, Language language, DateTimeOffset eventDate);

        Task<IEnumerable<MatchCommentary>> GetMatchCommentary(string id, Language language, DateTimeOffset eventDate);

        Task<MatchStatistic> GetMatchStatistic(string id, DateTimeOffset eventDate);

        Task<MatchLineups> GetMatchLineups(string id, Language language, DateTimeOffset eventDate);

        Task<IEnumerable<MatchSummary>> GetByIds(string[] ids, Language language);
    }

    public class MatchQueryService : IMatchQueryService
    {
        private const int MatchDataCacheInMinutes = 2;
        private const string MatchStatisticCacheKey = "MatchQuery_MatchStatisticCacheKey";
        private const string MatchInfoCacheKey = "MatchQuery_MatchInfoCacheKey";
        private const string MatchListCacheKey = "MatchQuery_MatchListCacheKey";
        private const string MatchCommentaryCacheKey = "MatchQuery_MatchCommentaryCacheKey";
        private const string MatchLineupCacheKey = "MatchQuery_MatchLineupCacheKey";
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
            var liveMatches = await dynamicRepository.FetchAsync<Match>(new GetLiveMatchesCriteria(language));

            return liveMatches.Select(m => new MatchSummary(m));
        }

        public async Task<int> GetLiveMatchCount(Language language)
            => (await GetLive(language)).Count();

        public async Task<IEnumerable<MatchSummary>> GetByDateRange(DateTimeOffset from, DateTimeOffset to, Language language)
        {
            var dateRanges = DateRangeHelper.GenerateDateRanges(from, to);
            var matchList = new List<MatchSummary>();

            foreach (var dateRange in dateRanges)
            {
                var matches = dateRange.IsCached
                    ? await GetOrSetAsync(
                        MatchListCacheKey,
                        dateRange.From,
                        dateRange.To,
                        () => dynamicRepository.FetchAsync<Match>(new GetMatchesByDateRangeCriteria(dateRange.From, dateRange.To, language)))
                    : await dynamicRepository.FetchAsync<Match>(new GetMatchesByDateRangeCriteria(dateRange.From, dateRange.To, language));

                matchList.AddRange(matches.Select(m => new MatchSummary(m)).ToList());
            }

            return matchList
                .OrderBy(match => match.LeagueOrder)
                .ThenBy(match => match.EventDate);
        }

        public async Task<MatchInfo> GetMatchInfo(string id, Language language, DateTimeOffset eventDate)
        {
            var cacheMatch = await cacheManager.GetAsync<MatchInfo>(BuildMatchInfoCacheKey(id));

            return cacheMatch ?? await GetAndCacheMatchInfo(id, language, eventDate);
        }

        public async Task<MatchCoverage> GetMatchCoverage(string id, Language language, DateTimeOffset eventDate)
        {
            var match = await dynamicRepository.GetAsync<Match>(new GetMatchByIdCriteria(id, language, eventDate));

            return match == null
                ? new MatchCoverage(id, default)
                : new MatchCoverage(match.Id, match.Coverage);
        }

        public async Task<IEnumerable<MatchCommentary>> GetMatchCommentary(string id, Language language, DateTimeOffset eventDate)
        {
            var timelineEvents = await cacheManager.GetOrSetAsync(
                $"{MatchCommentaryCacheKey}_{id}",
                async () => await dynamicRepository.FetchAsync<TimelineEvent>(new GetCommentaryCriteria(id, language, eventDate)),
                GetCacheOptions());

            return timelineEvents.Select(t => new MatchCommentary(t));
        }

        private CacheItemOptions GetCacheOptions(int cachedMinutes = MatchDataCacheInMinutes)
            => new CacheItemOptions().SetAbsoluteExpiration(dateTimeNowFunc().AddMinutes(cachedMinutes));

        public async Task<MatchStatistic> GetMatchStatistic(string id, DateTimeOffset eventDate)
            => await cacheManager.GetOrSetAsync(
                $"{MatchStatisticCacheKey}_{id}",
                async () => await GetMatchStatisticData(id, eventDate),
                GetCacheOptions());

        public async Task<MatchLineups> GetMatchLineups(string id, Language language, DateTimeOffset eventDate)
            => await cacheManager.GetOrSetAsync(
                $"{MatchLineupCacheKey}_{id}_{language.Value}",
                async () => await GetMatchLineupsData(id, language, eventDate),
                GetCacheOptions());

        internal async Task<MatchLineups> GetMatchLineupsData(string id, Language language, DateTimeOffset eventDate)
        {
            var matchLineups = await dynamicRepository.GetAsync<MatchLineups>(new GetMatchLineupsCriteria(id, language, eventDate));

            if (matchLineups == null)
            {
                return new MatchLineups();
            }

            var timelineEvents = (await dynamicRepository.FetchAsync<TimelineEvent>(new GetTimelineEventsCriteria(id, eventDate))).ToList();

            CombineTimelineEventsIntoLineups(matchLineups.Home, timelineEvents);
            CombineTimelineEventsIntoLineups(matchLineups.Away, timelineEvents);

            return matchLineups;
        }

        private static void CombineTimelineEventsIntoLineups(TeamLineups teamLineups, List<TimelineEvent> timelines)
        {
            foreach (var player in teamLineups.Players)
            {
                player.EventStatistic = BuildPlayerEventStatistic(timelines, player, IsPlayerHasTimelineEvent);
            }

            foreach (var player in teamLineups.Substitutions)
            {
                player.EventStatistic = BuildPlayerEventStatistic(timelines, player, IsSubstitutePlayerHasTimelineEvent);
            }

            teamLineups.SubstitutionEvents = timelines
                .Where(timeline => timeline.Type == EventType.Substitution && timeline.IsHome == teamLineups.IsHome)
                .OrderByDescending(timeline => timeline.Time)
                .Select(timeline => MapPlayerJerseyNumberForSubstitutionEvent(teamLineups, timeline));
        }

        private static TimelineEvent MapPlayerJerseyNumberForSubstitutionEvent(TeamLineups teamLineups, TimelineEvent timeline)
        {
            var playerIn = teamLineups.Substitutions.FirstOrDefault(player => player.Id == timeline?.PlayerIn?.Id);
            var timelinePlayerIn = playerIn == null ? timeline.PlayerIn : new Player(playerIn);

            var playerOut = teamLineups.Players.FirstOrDefault(player => player.Id == timeline?.PlayerOut?.Id);
            var timelinePlayerOut = playerOut == null ? timeline.PlayerOut : new Player(playerOut);

            return new TimelineEvent(
                        timeline.Id,
                        timeline.Type,
                        timeline.Time,
                        timeline.MatchTime,
                        timeline.StoppageTime,
                        timeline.Period,
                        timeline.PeriodType,
                        timeline.InjuryTimeAnnounced,
                        timelinePlayerOut,
                        timelinePlayerIn);
        }

        private static Dictionary<EventType, int> BuildPlayerEventStatistic(IList<TimelineEvent> timelineEvents, Player player, Func<Player, TimelineEvent, EventType, bool> isPlayerHasTimelineEvent)
        {
            var playerStatistic = new Dictionary<EventType, int>();

            foreach (var lineupsEvent in TeamLineups.LineupsEvents)
            {
                var events = timelineEvents.Where(tl => isPlayerHasTimelineEvent(player, tl, lineupsEvent)).ToList();

                if (events.Count == 0)
                {
                    continue;
                }

                if (lineupsEvent == EventType.ScoreChange)
                {
                    AddGoalEvents(playerStatistic, events);
                }
                else
                {
                    playerStatistic.Add(lineupsEvent, events.Count);
                }
            }

            return playerStatistic;
        }

        private static bool IsPlayerHasTimelineEvent(Player player, TimelineEvent tl, EventType lineupsEvent)
            => tl.Type == lineupsEvent
                && (tl.Player?.Id == player.Id || tl.PlayerOut?.Id == player.Id || tl.GoalScorer?.Id == player.Id);

        private static bool IsSubstitutePlayerHasTimelineEvent(Player player, TimelineEvent tl, EventType lineupsEvent)
           => tl.Type == lineupsEvent
               && (tl.Player?.Id == player.Id || tl.PlayerIn?.Id == player.Id || tl.GoalScorer?.Id == player.Id);

        private static void AddGoalEvents(IDictionary<EventType, int> playerStatistic, IList<TimelineEvent> timelineEvents)
        {
            var normalGoals = timelineEvents.Where(timelineEvent => timelineEvent.GoalScorer?.GetEventTypeFromGoalMethod() == EventType.ScoreChange).ToList();

            if (normalGoals.Count > 0)
            {
                playerStatistic.Add(EventType.ScoreChange, normalGoals.Count);
            }

            var ownGoals = timelineEvents.Where(timelineEvent => timelineEvent.GoalScorer?.GetEventTypeFromGoalMethod() == EventType.ScoreChangeByOwnGoal).ToList();

            if (ownGoals.Count > 0)
            {
                playerStatistic.Add(EventType.ScoreChangeByOwnGoal, ownGoals.Count);
            }

            var penaltyGoals = timelineEvents.Where(timelineEvent => timelineEvent.GoalScorer?.GetEventTypeFromGoalMethod() == EventType.ScoreChangeByPenalty).ToList();

            if (penaltyGoals.Count > 0)
            {
                playerStatistic.Add(EventType.ScoreChangeByPenalty, penaltyGoals.Count);
            }
        }

        internal async Task<MatchStatistic> GetMatchStatisticData(string id, DateTimeOffset eventDate)
        {
            var match = await dynamicRepository.GetAsync<Match>(new GetMatchByIdCriteria(id, Language.en_US, eventDate));

            if (match == null)
            {
                return new MatchStatistic();
            }

            var matchStatistic = new MatchStatistic(match);

            return matchStatistic;
        }

        private async Task<MatchInfo> GetAndCacheMatchInfo(string id, Language language, DateTimeOffset eventDate)
        {
            var match = await dynamicRepository.GetAsync<Match>(new GetMatchByIdCriteria(id, language, eventDate));

            if (match == null)
            {
                // TODO: Should fix here
                return null;
            }

            var timelineEvents = await dynamicRepository.FetchAsync<TimelineEvent>(new GetTimelineEventsCriteria(id, eventDate));
            var matchInfo = new MatchInfo(new MatchSummary(match), timelineEvents, match.Venue, match.Referee, match.Attendance);
            await cacheManager.SetAsync(
                BuildMatchInfoCacheKey(id), matchInfo, BuildCacheOptions(match.EventDate.DateTime));

            //TODO filter by highlight events

            return matchInfo;
        }

        private static string BuildMatchInfoCacheKey(string matchId)
            => $"{MatchInfoCacheKey}_{matchId}";

        private Task<T> GetOrSetAsync<T>(string key, DateTimeOffset from, DateTimeOffset to, Func<Task<T>> factory)
        {
            var cacheItemOptions = BuildCacheOptions(from);
            var cacheKey = BuildCacheKey(key, from, to);

            return cacheManager
                .GetOrSetAsync(cacheKey, factory, cacheItemOptions);
        }

        private CacheItemOptions BuildCacheOptions(DateTimeOffset date)
        {
            var cacheDuration = ShouldCacheWithShortDuration(date)
                ? appSettings.MatchShortCacheTimeDuration
                : appSettings.MatchLongCacheTimeDuration;

            return new CacheItemOptions().SetAbsoluteExpiration(DateTimeOffset.Now.AddSeconds(cacheDuration));
        }

        private bool ShouldCacheWithShortDuration(DateTimeOffset date)
            => date.ToUniversalTime().Date == dateTimeNowFunc().UtcDateTime.Date
               || date.ToUniversalTime().Date == dateTimeNowFunc().UtcDateTime.Date.AddDays(-1);

        private static string BuildCacheKey(string key, DateTimeOffset from, DateTimeOffset to)
            => $"{key}_{from.ToString(FormatDate)}_{to.ToString(FormatDate)}";

        public async Task<IEnumerable<MatchSummary>> GetByIds(string[] ids, Language language)
        {
            var matches = new List<MatchSummary>();

            var aheadMatches = dynamicRepository
                .FetchAsync<Match>(new GetMatchesByIdsCriteria(
                    ids,
                    language,
                    DateTimeOffset.Now.AddDays(appSettings.DatabaseQueryDateSpan)));

            var currentMatches = dynamicRepository
                .FetchAsync<Match>(new GetMatchesByIdsCriteria(
                    ids,
                    language));

            var results = await Task.WhenAll(currentMatches, aheadMatches);

            foreach (var result in results)
            {
                matches.AddRange(result.Select(m => new MatchSummary(m)));
            }

            return matches;
        }
    }
}
﻿[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Soccer.API.Tests")]

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
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Shared.Enumerations;
    using Soccer.Core.Teams.Models;
    using Soccer.Database.Matches.Criteria;
    using Soccer.Database.Timelines.Criteria;

    public interface IMatchQueryService
    {
        Task<IEnumerable<MatchSummary>> GetByDateRange(DateTime from, DateTime to, Language language);

        Task<MatchInfo> GetMatchInfo(string id, Language language);

        Task<IEnumerable<MatchSummary>> GetLive(Language language);

        Task<int> GetLiveMatchCount(Language language);

        Task<MatchCoverage> GetMatchCoverage(string id, Language language);

        Task<IEnumerable<MatchCommentary>> GetMatchCommentary(string id, Language language);

        Task<MatchStatistic> GetMatchStatistic(string id);

        Task<MatchLineups> GetMatchLineups(string id, Language language);
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

            return cacheMatch ?? await GetAndCacheMatchInfo(id, language);
        }

        public async Task<MatchCoverage> GetMatchCoverage(string id, Language language)
        {
            var match = await dynamicRepository.GetAsync<Match>(new GetMatchByIdCriteria(id, language));

            return new MatchCoverage(match.Id, match.Coverage);
        }

        public async Task<IEnumerable<MatchCommentary>> GetMatchCommentary(string id, Language language)
        {
            var timelineEvents = await cacheManager.GetOrSetAsync(
                $"{MatchCommentaryCacheKey}_{id}",
                async () => await dynamicRepository.FetchAsync<TimelineEvent>(new GetCommentaryCriteria(id, language)),
                GetCacheOptions());

            return timelineEvents.Select(t => new MatchCommentary(t));
        }

        private CacheItemOptions GetCacheOptions(int cachedMinutes = MatchDataCacheInMinutes)
            => new CacheItemOptions().SetAbsoluteExpiration(dateTimeNowFunc().AddMinutes(cachedMinutes));

        public async Task<MatchStatistic> GetMatchStatistic(string id)
            => await cacheManager.GetOrSetAsync(
                $"{MatchStatisticCacheKey}_{id}",
                async () => await GetMatchStatisticData(id),
                GetCacheOptions());

        public async Task<MatchLineups> GetMatchLineups(string id, Language language)
            => await cacheManager.GetOrSetAsync(
                $"{MatchLineupCacheKey}_{id}_{language.Value}",
                async () => await GetMatchLineupsData(id, language),
                GetCacheOptions(0));

        internal async Task<MatchLineups> GetMatchLineupsData(string id, Language language)
        {
            var matchLineups = await dynamicRepository.GetAsync<MatchLineups>(new GetMatchLineupsCriteria(id, language));

            if (matchLineups == null)
            {
                return new MatchLineups();
            }

            var timelines = (await dynamicRepository.FetchAsync<TimelineEvent>(new GetTimelineEventsCriteria(id))).ToList();

            CombineTimelineEventsIntoLineups(matchLineups.Home, timelines);
            CombineTimelineEventsIntoLineups(matchLineups.Away, timelines);

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
            var playerIn = teamLineups.Substitutions.FirstOrDefault(player => player.Id == timeline.PlayerIn.Id);
            var timelinePlayerIn = playerIn == null ? timeline.PlayerIn : new Player(playerIn);

            var playerOut = teamLineups.Players.FirstOrDefault(player => player.Id == timeline.PlayerOut.Id);
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

        private static Dictionary<EventType, int> BuildPlayerEventStatistic(List<TimelineEvent> timelines, Player player, Func<Player, TimelineEvent, EventType, bool> isPlayerHasTimelineEvent)
        {
            var playerStatistic = new Dictionary<EventType, int>();

            foreach (var lineupsEvent in TeamLineups.LineupsEvents)
            {
                var events = timelines.Where(tl => isPlayerHasTimelineEvent(player, tl, lineupsEvent));

                if (events.Any())
                {
                    if (lineupsEvent == EventType.ScoreChange)
                    {
                        AddGoalEvents(playerStatistic, events);
                    }
                    else
                    {
                        playerStatistic.Add(lineupsEvent, events.Count());
                    }
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

        private static void AddGoalEvents(Dictionary<EventType, int> playerStatistic, IEnumerable<TimelineEvent> timelineEvents)
        {
            var normalGoals = timelineEvents.Where(timelineEvent => timelineEvent.GoalScorer?.GetEventTypeFromGoalMethod() == EventType.ScoreChange);
            if (normalGoals.Any())
            {
                playerStatistic.Add(EventType.ScoreChange, normalGoals.Count());
            }

            var ownGoals = timelineEvents.Where(timelineEvent => timelineEvent.GoalScorer?.GetEventTypeFromGoalMethod() == EventType.ScoreChangeByOwnGoal);
            if (ownGoals.Any())
            {
                playerStatistic.Add(EventType.ScoreChangeByOwnGoal, ownGoals.Count());
            }

            var penaltyGoals = timelineEvents.Where(timelineEvent => timelineEvent.GoalScorer?.GetEventTypeFromGoalMethod() == EventType.ScoreChangeByPenalty);
            if (penaltyGoals.Any())
            {
                playerStatistic.Add(EventType.ScoreChangeByPenalty, penaltyGoals.Count());
            }
        }

        private async Task<MatchStatistic> GetMatchStatisticData(string id)
        {
            var match = await dynamicRepository.GetAsync<Match>(new GetMatchByIdCriteria(id, Language.en_US));

            if (match == null)
            {
                return new MatchStatistic();
            }

            var matchStatistic = new MatchStatistic(match);

            return matchStatistic;
        }

        private async Task<MatchInfo> GetAndCacheMatchInfo(string id, Language language)
        {
            var match = await dynamicRepository.GetAsync<Match>(new GetMatchByIdCriteria(id, language));

            if (match == null)
            {
                // TODO: Should fix here
                return null;
            }

            var timelineEvents = await dynamicRepository.FetchAsync<TimelineEvent>(new GetTimelineEventsCriteria(id));
            var matchInfo = new MatchInfo(new MatchSummary(match), timelineEvents, match.Venue, match.Referee, match.Attendance);
            await cacheManager.SetAsync(
                BuildMatchInfoCacheKey(id), matchInfo, BuildCacheOptions(match.EventDate.DateTime));

            //TODO filter by highlight events

            return matchInfo;
        }

        private static string BuildMatchInfoCacheKey(string matchId)
            => $"{MatchInfoCacheKey}_{matchId}";

        private Task<T> GetOrSetAsync<T>(string key, DateTime from, DateTime to, Func<Task<T>> factory)
        {
            var cacheItemOptions = BuildCacheOptions(from);
            var cacheKey = BuildCacheKey(key, from, to);

            return cacheManager
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
    }
}
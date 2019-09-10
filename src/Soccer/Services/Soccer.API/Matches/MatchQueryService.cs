namespace Soccer.API.Matches
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Soccer.API.Matches.Models;
    using Soccer.API.Shared.Services;
    using Soccer.Core.Leagues.Models;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Shared.Enumerations;
    using Soccer.Core.Teams.Models;
    using Soccer.Database.Matches.Criteria;

    public interface IMatchQueryService
    {
        Task<IEnumerable<MatchSummary>> GetByDateRange(DateTime from, DateTime to, Language language);

        Task<MatchInfo> GetMatchInfo(string id, Language language);

        Task<IEnumerable<Match>> GetLive(TimeSpan clientTimeOffset, Language language);
    }

    public class MatchQueryService : IMatchQueryService
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly IDateCacheService dateCacheService;

        public MatchQueryService(
            IDynamicRepository dynamicRepository,
            IDateCacheService dateCacheService)
        {
            this.dynamicRepository = dynamicRepository;
            this.dateCacheService = dateCacheService;
        }

        public async Task<IEnumerable<Match>> GetLive(TimeSpan clientTimeOffset, Language language)
            => await dynamicRepository.FetchAsync<Match>(new GetLiveMatchesCriteria(language));

        public async Task<IEnumerable<MatchSummary>> GetByDateRange(DateTime from, DateTime to, Language language)
        {
            var cachedMatches = await dateCacheService.GetOrSetAsync(
                nameof(GetByDateRange),
                from,
                to,
                () =>
                {
                    var matches = dynamicRepository.Fetch<Match>(new GetMatchesByDateRangeCriteria(from, to, language));

                    return matches.Select(m => new MatchSummary(m));
                });

            return cachedMatches;
        }

        public async Task<MatchInfo> GetMatchInfo(string id, Language language)
        {
            var matchInfo = CreateMatchInfo();

            return await Task.FromResult(matchInfo);

            //var match = await dynamicRepository.GetAsync<Match>(new GetMatchByIdCriteria(id, language));

            //if (match != null)
            //{
            //    var timelineEvents = await dynamicRepository.FetchAsync<TimelineEvent>(new GetTimelineEventsCriteria(id));

            //    return new MatchInfo(match, timelineEvents);
            //}

            //return null;
        }

        public static MatchInfo CreateMatchInfo()
        {
            var match = CreateMatch();
            return new MatchInfo(new MatchSummary(match), new List<TimelineEvent> { CreateNewEvent() }, match.Venue, match.Referee, match.Attendance);
        }

        private static TimelineEvent CreateNewEvent()
        {
            return new TimelineEvent
            {
                Id = "223232",
                Type = EventType.ScoreChange,
                Team = "Team score change",
                Assist = CreatePlayer("Assist"),
                Name = DateTimeOffset.Now.ToString(),
                AwayScore = 1,
                HomeScore = 3,
                AwayShootoutPlayer = CreatePlayer("AwayShootoutPlayer"),
                Outcome = "outcom",
                Player = CreatePlayer("Player"),
                Description = "3434",
                GoalScorer = new GoalScorer { Id = "223423", Name = "GoalScorer", Method = "Method" },
                MatchTime = 22,
                MatchClock = "343:34",
                Time = DateTimeOffset.Now,
                ShootoutAwayScore = 2,
                IsHomeShootoutScored = true,
                ShootoutHomeScore = 3,
                IsAwayShootoutScored = false,
                InjuryTimeAnnounced = 22,
                HomeShootoutPlayer = CreatePlayer("HomeShootoutPlayer"),
                StoppageTime = "3434",
                PeriodType = PeriodType.AwaitingExtraTime,
                Period = 1,
                ModifiedTime = DateTimeOffset.Now,
                PenaltyStatus = "go",
                IsFirstShoot = false,
                TestString = DateTimeOffset.Now.ToString()
            };
        }

        private static Match CreateMatch()
        {
            return new Match
            {
                Attendance = 1,
                CreatedTime = DateTimeOffset.Now,
                CurrentPeriodStartTime = DateTimeOffset.Now,
                EventDate = DateTimeOffset.Now,
                Id = "1212112",
                LatestTimeline = new TimelineEvent { Id = "12", CreatedTime = DateTimeOffset.Now, Assist = new Player { Id = "23232", JerseyNumber = 111, Name = "PlayerName", Order = 1, Position = "3434", Type = "3434" } },
                League = new League { Id = "3333", Order = 1, Name = "league name", Flag = "Flag", Category = new LeagueCategory { Id = "LeagueCategory", Name = "LeagueCategory NAme", CountryCode = "LeagueCategory CTX" } },
                LeagueRound = new LeagueRound { Name = "LeagueRound name", Group = "LeagueRound Group", Number = 1, Phase = "LeagueRound Phase", Type = LeagueRoundType.PlayOffRound },
                MatchResult = new MatchResult
                {
                    AggregateAwayScore = 1,
                    AggregateHomeScore = 2,
                    AggregateWinnerId = "Home",
                    AwayScore = 1,
                    EventStatus = MatchStatus.Live,
                    HomeScore = 2,
                    WinnerId = "winner id",
                    MatchStatus = MatchStatus.Live,
                    MatchPeriods = new List<MatchPeriod> { new MatchPeriod { HomeScore = 1, AwayScore = 2, Number = 1, PeriodType = PeriodType.RegularPeriod } }
                },
                ModifiedTime = DateTimeOffset.Now,
                Referee = "Referee",
                Region = "Region",
                Teams = new List<Team> { new Team { Id = "3434", Name = "name", } },
                Venue = new Venue { Id = "Venue Id", Capacity = 343434, CityName = "City Name", CountryCode = "Country Name", Name = "Venue name", CountryName = "Country name" }
            };
        }

        public static Player CreatePlayer(string name)
            => new Player
            {
                Id = "232",
                Name = "Player Name" + name,
                JerseyNumber = 343,
                Order = 1,
                Position = "GK" + name,
                Type = "Player type" + name
            };
    }
}
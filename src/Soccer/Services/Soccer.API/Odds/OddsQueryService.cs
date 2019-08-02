namespace Soccer.API.Odds
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Soccer.API.Matches;
    using Soccer.Core._Shared.Resources;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Odds.Models;
    using Soccer.Core.Shared.Enumerations;
    using Soccer.Database.Odds.Criteria;

    public interface IOddsQueryService
    {
        Task<MatchOdds> GetOdds(string matchId, int betTypeId, Language language);

        Task<MatchOddsMovement> GetOddsMovement(string matchId, int betTypeId, string bookmakerId, Language language);
    }

    public class OddsQueryService : IOddsQueryService
    {
        private const int firstPeriod = 1;
        private const int secondPeriod = 2;
        private const int startSecondHaft = 46;
        private const string pause = "pause";
        private readonly IDynamicRepository dynamicRepository;
        private readonly IMatchQueryService matchQueryService;

        public OddsQueryService(
            IDynamicRepository dynamicRepository,
            IMatchQueryService matchQueryService)
        {
            this.dynamicRepository = dynamicRepository;
            this.matchQueryService = matchQueryService;
        }

        public async Task<MatchOdds> GetOdds(string matchId, int betTypeId, Language language)
            => new MatchOdds(matchId, await GetBookmakerComparisonOdds(matchId, betTypeId));

        private async Task<IEnumerable<BetTypeOdds>> GetBookmakerComparisonOdds(string matchId, int betTypeId)
        {
            var oddsByBookmaker = (await GetOddsData(matchId, betTypeId)).GroupBy(o => o.Bookmaker?.Id);

            var betTypeOdssList = oddsByBookmaker
                .Select(group =>
                {
                    var first = group.First();
                    var last = group.Last();

                    first.AssignOpeningData(last.BetOptions);

                    return first;
                })
                .OrderBy(bto => bto.Bookmaker.Name)
                .AsEnumerable();

            return betTypeOdssList;
        }

        // TODO: Write SP & Implement get data code here
        private async Task<IEnumerable<BetTypeOdds>> GetOddsData(string matchId, int betTypeId)
            => await dynamicRepository.FetchAsync<BetTypeOdds>(new GetOddsCriteria(matchId, betTypeId));

        public async Task<MatchOddsMovement> GetOddsMovement(string matchId, int betTypeId, string bookmakerId, Language language)
        {
            var betTypeOddsList = await GetBookmakerOddsListByBetType(/*matchId, betTypeId, bookmakerId*/);
            var firstOdds = betTypeOddsList.FirstOrDefault();

            if (firstOdds == null)
            {
                return new MatchOddsMovement();
            }

            return new MatchOddsMovement(matchId, firstOdds.Bookmaker, await BuildOddsMovement(matchId, betTypeOddsList, language));
        }

        // TODO: Write SP & Implement get data code here
        private static Task<List<BetTypeOdds>> GetBookmakerOddsListByBetType(/*string matchId, int betTypeId, string bookmakerId*/)
            => Task.FromResult(new List<BetTypeOdds>());

        private static readonly IList<byte> OddChangeEventIds
            = new List<byte>
            {
                EventType.PeriodStart.Value,
                EventType.ScoreChange.Value,
                EventType.MatchEnded.Value,
                EventType.BreakStart.Value
            };

        private async Task<IEnumerable<OddsMovement>> BuildOddsMovement(
            string matchId,
            List<BetTypeOdds> betTypeOddsList,
            Language language)
        {
            var match = await matchQueryService.GetMatch(matchId, language);

            if (match == null)
            {
                return Enumerable.Empty<OddsMovement>();
            }

            var oddsMovements = BuildOddMovementBeforeMatchStart(betTypeOddsList, match)
                .Concat(BuildOddMovementAfterMatchStarted(betTypeOddsList, match));

            CalculateOddsTrend(oddsMovements);

            return oddsMovements.Reverse();
        }

        private static IEnumerable<TimelineEventEntity> GetMainEvents(Match match)
            => match.TimeLines.Where(tl
#pragma warning disable S1067 // Expressions should not be too complex
                 => tl != null
                    && OddChangeEventIds.Contains(tl.Type.Value)
                    && (tl.PeriodType == PeriodType.RegularPeriod
                            || tl.PeriodType.DisplayName == pause));

#pragma warning restore S1067 // Expressions should not be too complex

        private static void CalculateOddsTrend(IEnumerable<OddsMovement> oddsMovements)
        {
            OddsMovement prevOdds = null;
            foreach (var oddsMovement in oddsMovements)
            {
                if (prevOdds == null)
                {
                    prevOdds = oddsMovement;
                }
                else
                {
                    oddsMovement.CalculateOddsTrend(prevOdds.BetOptions);
                    prevOdds = oddsMovement;
                }
            }
        }

        private static IEnumerable<OddsMovement> BuildOddMovementAfterMatchStarted(
            IEnumerable<BetTypeOdds> betTypeOddsList,
            Match match)
        {
            if (match.TimeLines == null)
            {
                return Enumerable.Empty<OddsMovement>();
            }

            var homeScore = 0;
            var awayScore = 0;
            TimelineEventEntity currentEvent = null;
            var timelineEvents = GetMainEvents(match);
            var oddsMovements = new List<OddsMovement>();
            var matchLiveOdds = betTypeOddsList.Where(o => o.LastUpdatedTime >= match.EventDate);

            foreach (var betTypeOdds in matchLiveOdds)
            {
                var timelineEvent = timelineEvents.FirstOrDefault(e => e.Time == betTypeOdds.LastUpdatedTime);
                var oddsMovement = BuildOddsMovementEvent(ref homeScore, ref awayScore, ref currentEvent, timelineEvent, betTypeOdds);

                oddsMovements.Add(oddsMovement);
            }

            return oddsMovements;
        }

        private static OddsMovement BuildOddsMovementEvent(
            ref int homeScore,
            ref int awayScore,
            ref TimelineEventEntity currentEvent,
            TimelineEventEntity timelineEvent,
            BetTypeOdds betTypeOdds)
        {
            var matchTime = string.Empty;

            if (IsScoreChange(timelineEvent))
            {
                homeScore = timelineEvent.HomeScore;
                awayScore = timelineEvent.AwayScore;
                matchTime = $"{timelineEvent.MatchTime}'";
            }

            if (IsHalfTimeBreakStart(timelineEvent))
            {
                currentEvent = timelineEvent;
                matchTime = AppResources.HT;
            }

            if (IsFirstPeriodStart(timelineEvent))
            {
                currentEvent = timelineEvent;
                matchTime = AppResources.KO;
            }

            if (IsSecondPeriodStart(timelineEvent))
            {
                currentEvent = timelineEvent;
                currentEvent.Time = currentEvent.Time.AddMinutes(-startSecondHaft);
            }

            if (string.IsNullOrWhiteSpace(matchTime))
            {
                var totalMinutes = (betTypeOdds.LastUpdatedTime - currentEvent.Time).TotalMinutes;
                matchTime = totalMinutes.ToString("0") + "'";
            }

            return new OddsMovement(
                betTypeOdds.BetOptions,
                matchTime,
                betTypeOdds.LastUpdatedTime,
                currentEvent != null,
                homeScore,
                awayScore);
        }

        private static bool IsScoreChange(TimelineEventEntity timelineEvent)
            => timelineEvent != null
                && timelineEvent.Type == EventType.ScoreChange;

        private static bool IsHalfTimeBreakStart(TimelineEventEntity timelineEvent)
            => timelineEvent != null
                && timelineEvent.Type == EventType.BreakStart
                && timelineEvent.PeriodType.DisplayName.ToLowerInvariant() == pause;

        private static bool IsSecondPeriodStart(TimelineEventEntity timelineEvent)
            => timelineEvent != null
                && timelineEvent.Type == EventType.PeriodStart
                && timelineEvent.Period == secondPeriod
                && timelineEvent.PeriodType == PeriodType.RegularPeriod;

        private static bool IsFirstPeriodStart(TimelineEventEntity timelineEvent)
            => timelineEvent != null
                && timelineEvent.Type == EventType.PeriodStart
                && timelineEvent.Period == firstPeriod
                && timelineEvent.PeriodType == PeriodType.RegularPeriod;

        private static IEnumerable<OddsMovement> BuildOddMovementBeforeMatchStart(
            List<BetTypeOdds> betTypeOddsList,
            Match match)
        {
            var firstBetTypeOdds = betTypeOddsList.FirstOrDefault();
            var oddsMovements = new List<OddsMovement>
            {
                BuildOpeningOddsMovement(firstBetTypeOdds)
            }
            .Concat(BuildLiveOddsMovement(betTypeOddsList, match, firstBetTypeOdds));

            return oddsMovements;
        }

        private static IEnumerable<OddsMovement> BuildLiveOddsMovement(List<BetTypeOdds> betTypeOddsList, Match match, BetTypeOdds firstBetTypeOdds)
        {
            var liveOddsMovement = new List<OddsMovement>();
            var beforeLiveMatchOdds = betTypeOddsList.Where(
                            o => o.LastUpdatedTime < match.EventDate
                                && o.LastUpdatedTime > firstBetTypeOdds.LastUpdatedTime);

            foreach (var betTypeOdds in beforeLiveMatchOdds)
            {
                var oddsMovement = new OddsMovement(betTypeOdds.BetOptions, AppResources.Live, betTypeOdds.LastUpdatedTime);

                liveOddsMovement.Add(oddsMovement);
            }

            return liveOddsMovement;
        }

        private static OddsMovement BuildOpeningOddsMovement(BetTypeOdds firstBetTypeOdds)
        {
            var openingOddsMovement = new OddsMovement(firstBetTypeOdds.BetOptions, AppResources.Opening, firstBetTypeOdds.LastUpdatedTime);

            openingOddsMovement.ResetLiveOddsToOpeningOdds();

            return openingOddsMovement;
        }
    }
}
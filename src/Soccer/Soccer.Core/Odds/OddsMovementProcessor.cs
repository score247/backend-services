namespace Soccer.Core.Odds
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Soccer.Core._Shared.Resources;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Odds.Models;
    using Soccer.Core.Shared.Enumerations;

    public static class OddsMovementProcessor
    {
        private const int firstPeriod = 1;
        private const int secondPeriod = 2;
        private const int twoItems = 2;
        private const int secondItemIndex = 1;
        private const int startSecondHaft = 46;
        private const string pause = "pause";

        public static IEnumerable<OddsMovement> BuildOddsMovements(
            Match match,
            List<BetTypeOdds> betTypeOddsList,
            int numOfDaysToShowOddsBeforeKickoffDate)
        {
            var firstBetTypeOdds = betTypeOddsList.FirstOrDefault();

            if (match == null || firstBetTypeOdds == null)
            {
                return Enumerable.Empty<OddsMovement>();
            }

            var minDate = match.EventDate.AddDays(-numOfDaysToShowOddsBeforeKickoffDate).Date;
            var filteredBetTypeOddsList = betTypeOddsList.Where(
                bto => bto.LastUpdatedTime == firstBetTypeOdds.LastUpdatedTime
                        || bto.LastUpdatedTime >= minDate).ToList();

            var oddsMovements = BuildOddMovementBeforeMatchStart(firstBetTypeOdds, filteredBetTypeOddsList, match)
                .Concat(BuildOddMovementAfterMatchStarted(filteredBetTypeOddsList, match));

            CalculateOddsTrend(oddsMovements);

            return oddsMovements.Reverse();
        }

        public static BetTypeOdds AssignOpeningOddsToFirstOdds(IEnumerable<BetTypeOdds> betTypeOddList, DateTime minDate)
        {
            var orderedBetTypeOddsList = betTypeOddList.OrderByDescending(bto => bto.LastUpdatedTime).AsEnumerable();
            var last = orderedBetTypeOddsList.Last();

            orderedBetTypeOddsList = orderedBetTypeOddsList.Where(bto => bto.LastUpdatedTime == last.LastUpdatedTime || bto.LastUpdatedTime >= minDate);

            var totalItems = orderedBetTypeOddsList.Count();
            var first = orderedBetTypeOddsList.First();

            first.AssignOpeningData(last.BetOptions);

            if (totalItems > twoItems)
            {
                var secondItem = orderedBetTypeOddsList.ElementAt(secondItemIndex);
                CalculateOddsTrend(first.BetOptions, secondItem.BetOptions);
            }

            return first;
        }

        private static readonly IList<byte> OddChangeEventIds
            = new List<byte>
            {
                 EventType.PeriodStart.Value,
                 EventType.ScoreChange.Value,
                 EventType.MatchEnded.Value,
                 EventType.BreakStart.Value
            };

        private static IEnumerable<TimelineEvent> GetMainEvents(Match match)
            => match.TimeLines.Where(tl
#pragma warning disable S1067 // Expressions should not be too complex
                 => IsTimelineNeedMapWithOddsData(tl)).OrderBy(t => t.Time);

        public static bool IsTimelineNeedMapWithOddsData(TimelineEvent tl)
        {
            var needMap = tl != null
                                && tl.Type != null
                                && OddChangeEventIds.Contains(tl.Type.Value)
                                && tl.PeriodType != null
                                && (tl.PeriodType == PeriodType.RegularPeriod
                                        || tl.PeriodType.DisplayName == pause);

            return needMap;
        }

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
                    CalculateOddsTrend(oddsMovement.BetOptions, prevOdds.BetOptions);
                    prevOdds = oddsMovement;
                }
            }
        }

        public static void CalculateOddsTrend(
            IEnumerable<BetOptionOdds> currentBetOptions,
            IEnumerable<BetOptionOdds> prevBetOptions)
        {
            if (prevBetOptions.Count() != currentBetOptions.Count())
            {
                return;
            }

            var totalBetOptions = prevBetOptions.Count();

            for (int i = 0; i < totalBetOptions; i++)
            {
                currentBetOptions.ElementAt(i).CalculateOddsTrend(prevBetOptions.ElementAt(i).LiveOdds);
            }
        }

        private static IEnumerable<OddsMovement> BuildOddMovementAfterMatchStarted(
            IEnumerable<BetTypeOdds> betTypeOddsList,
            Match match)
        {
            if (match.TimeLines == null
                || !match.TimeLines.Any())
            {
                return Enumerable.Empty<OddsMovement>();
            }

            var homeScore = 0;
            var awayScore = 0;
            TimelineEvent currentEvent = null;
            var timelineEvents = GetMainEvents(match);
            var oddsMovements = new List<OddsMovement>();
            var matchLiveOdds = betTypeOddsList.Where(o => o.LastUpdatedTime >= match.EventDate);
            var currentMatchTime = DateTimeOffset.MinValue;

            foreach (var betTypeOdds in matchLiveOdds)
            {
                var timelineEvent = timelineEvents.FirstOrDefault(e => e.Time == betTypeOdds.LastUpdatedTime);
                var oddsMovement = BuildOddsMovementEvent(ref homeScore, ref awayScore, ref currentEvent, ref currentMatchTime, timelineEvent, betTypeOdds);

                oddsMovements.Add(oddsMovement);
            }

            return oddsMovements;
        }

        private static OddsMovement BuildOddsMovementEvent(
            ref int homeScore,
            ref int awayScore,
            ref TimelineEvent currentEvent,
            ref DateTimeOffset currentMatchTime,
            TimelineEvent timelineEvent,
            BetTypeOdds betTypeOdds)
        {
            var matchTime = string.Empty;

            if (timelineEvent != null)
            {
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
                    currentMatchTime = timelineEvent.Time;
                }

                if (IsSecondPeriodStart(timelineEvent))
                {
                    currentEvent = timelineEvent;
                    currentMatchTime = timelineEvent.Time.AddMinutes(-startSecondHaft);
                }
            }

            if (string.IsNullOrWhiteSpace(matchTime)
                && currentEvent != null)
            {
                if (IsHalfTimeBreakStart(currentEvent))
                {
                    matchTime = AppResources.HT;
                }
                else
                {
                    var totalMinutes = (betTypeOdds.LastUpdatedTime - currentMatchTime).TotalMinutes;
                    matchTime = totalMinutes.ToString("0") + "'";
                }
            }

            return new OddsMovement(
                betTypeOdds.BetOptions,
                matchTime,
                betTypeOdds.LastUpdatedTime,
                currentEvent != null,
                homeScore,
                awayScore);
        }

        private static bool IsScoreChange(TimelineEvent timelineEvent)
            => timelineEvent != null
                && timelineEvent.Type == EventType.ScoreChange;

        private static bool IsHalfTimeBreakStart(TimelineEvent timelineEvent)
            => timelineEvent != null
                && timelineEvent.Type == EventType.BreakStart
                && timelineEvent.PeriodType.DisplayName.ToLowerInvariant() == pause;

        private static bool IsSecondPeriodStart(TimelineEvent timelineEvent)
            => timelineEvent != null
                && timelineEvent.Type == EventType.PeriodStart
                && timelineEvent.Period == secondPeriod
                && timelineEvent.PeriodType == PeriodType.RegularPeriod;

        private static bool IsFirstPeriodStart(TimelineEvent timelineEvent)
            => timelineEvent != null
                && timelineEvent.Type == EventType.PeriodStart
                && timelineEvent.Period == firstPeriod
                && timelineEvent.PeriodType == PeriodType.RegularPeriod;

        private static IEnumerable<OddsMovement> BuildOddMovementBeforeMatchStart(
            BetTypeOdds firstBetTypeOdds,
            List<BetTypeOdds> betTypeOddsList,
            Match match)
        {
            var oddsMovements = new List<OddsMovement>
            {
                BuildOpeningOddsMovement(firstBetTypeOdds)
            }
            .Concat(BuildLiveOddsMovement(betTypeOddsList, match, firstBetTypeOdds));

            return oddsMovements;
        }

        private static IEnumerable<OddsMovement> BuildLiveOddsMovement(
            List<BetTypeOdds> betTypeOddsList,
            Match match,
            BetTypeOdds firstBetTypeOdds)
        {
            var liveOddsMovement = new List<OddsMovement>();
            var beforeLiveMatchOdds = betTypeOddsList.Where(
                            o => o.LastUpdatedTime < match.EventDate
                                && o.LastUpdatedTime > firstBetTypeOdds.LastUpdatedTime);

            foreach (var betTypeOdds in beforeLiveMatchOdds)
            {
                var oddsMovement = new OddsMovement(
                    betTypeOdds.BetOptions,
                    AppResources.Live,
                    betTypeOdds.LastUpdatedTime);

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
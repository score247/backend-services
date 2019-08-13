namespace Soccer.DataProviders.SportRadar.Odds.DataMappers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Soccer.Core._Shared.Resources;
    using Soccer.Core.Odds.Models;
    using Soccer.DataProviders.SportRadar.Odds.Dtos;

    public static class OddsMapper
    {
        private const string UnSupportedBetTypeName = "Un-supported Bet Type";
        private const int OneXTwoBetTypeId = 1; //2, 3way
        private const string OneXTwoSportRadarBetTypeName = "3way";
        private const int OverUnderBeTypeId = 2; //3, total
        private const string OverUnderSportRadarBetTypeName = "total";
        private const int AsianHandicapBetTypeId = 3;// 5, asian_handicap
        private const string AsianHandicapSportRadarBetTypeName = "asian_handicap";
        private const int UnknownBetTypeId = 1000;
        private const string RegularGroupName = "regular";
        private const int numOfRounding = 2;

        public static IEnumerable<MatchOdds> MapToMatchOddsList(OddsScheduleDto oddsScheduleDto)
        {
            if (oddsScheduleDto == null
                || oddsScheduleDto.sport_events == null
                || !oddsScheduleDto.sport_events.Any())
            {
                return Enumerable.Empty<MatchOdds>();
            }

            var matchOddsList = new List<MatchOdds>();

            foreach (var sportEvent in oddsScheduleDto.sport_events)
            {
                matchOddsList.Add(MapToMatchOdds(sportEvent));
            }

            return matchOddsList;
        }

        public static MatchOdds MapToMatchOdds(SportEvent sport_event)
        {
            var betTypeOddsList = new List<BetTypeOdds>();
            if (sport_event.markets != null
                && sport_event.markets.Any())
            {
                foreach (var market in sport_event.markets)
                {
                    BuildBetTypeOdds(betTypeOddsList, market);
                }
            }

            return new MatchOdds(sport_event.id, betTypeOddsList, sport_event.markets_last_updated);
        }

        private static void BuildBetTypeOdds(List<BetTypeOdds> betTypeOddsList, Market market)
        {
            if (market.group_name?.ToLowerInvariant() == RegularGroupName)
            {
                var betType = GetBetType(market);

                if (betType.Item1 != UnknownBetTypeId)
                {
                    foreach (var book in market.books)
                    {
                        betTypeOddsList.Add(BuildBetTypeOdds(betType, book));
                    }
                }
            }
        }

        private static BetTypeOdds BuildBetTypeOdds((int, string) betType, Book book)
            => new BetTypeOdds(
                betType.Item1,
                betType.Item2,
                new Bookmaker(book.id, book.name),
                DateTime.Now,
                book.outcomes?.Select(oc => BuildBetOption(betType, oc)));

        private static BetOptionOdds BuildBetOption((int, string) betType, Outcome oc)
        {
            var liveOdds = oc.odds == null
                                    ? 0
                                    : Round(oc.odds);

            var openingOdds = oc.opening_odds == null
                                    ? liveOdds
                                    : Round(oc.opening_odds);

            var openingOptionValue = GetOpeningOptionValue(betType.Item1, oc);

            return new BetOptionOdds(
                    oc.type,
                    liveOdds,
                    openingOdds,
                    GetOptionValue(betType.Item1, oc),
                    openingOptionValue
                );
        }

        private static decimal Round(string odds)
            => Math.Round(decimal.Parse(odds), numOfRounding, MidpointRounding.AwayFromZero);

        private static string GetOptionValue(int betTypeId, Outcome oc)
        {
            switch (betTypeId)
            {
                case AsianHandicapBetTypeId:
                    return oc.handicap;

                case OverUnderBeTypeId:
                    return oc.total;

                default:
                    return string.Empty;
            }
        }

        private static string GetOpeningOptionValue(int betTypeId, Outcome oc)
            => betTypeId == OverUnderBeTypeId
                ? oc.opening_total?.ToString()
                : string.Empty;

        private static readonly IDictionary<string, (int, string)> BetTypeConverter = new Dictionary<string, (int, string)>
        {
            { OneXTwoSportRadarBetTypeName, (OneXTwoBetTypeId, AppResources.OneXTwo) },
            { OverUnderSportRadarBetTypeName, (OverUnderBeTypeId, AppResources.OverUnder) },
            { AsianHandicapSportRadarBetTypeName, (AsianHandicapBetTypeId, AppResources.Handicap) }
        };

        private static (int, string) GetBetType(Market market)
            => BetTypeConverter.ContainsKey(market.name)
                ? BetTypeConverter[market.name]
                : (UnknownBetTypeId, UnSupportedBetTypeName);
    }
}
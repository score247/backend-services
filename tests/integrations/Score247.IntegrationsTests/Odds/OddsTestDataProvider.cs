namespace Score247.IntegrationsTests.Odds
{
    using System;
    using System.Collections.Generic;
    using Soccer.DataProviders.SportRadar.Odds.Dtos;

    public static class OddsTestDataProvider
    {
        public static SportEvent Create1x2SportEvent(
            string matchId,
            DateTime? lastUpdated = null,
            List<Book> books = null)
            => new SportEvent
            {
                id = matchId,
                markets = new List<Market>
                {
                            new Market
                            {
                                odds_type_id = 2,
                                name = "3way",
                                group_name = "regular",
                                books = books ?? new List<Book>
                                {
                                    CreateOneXTwoBook("sr:book:1", "Unibet 1"),
                                    CreateOneXTwoBook("sr:book:2", "Unibet 2"),
                                    CreateOneXTwoBook("sr:book:3", "Unibet 3")
                                }
                            }
                },
                markets_last_updated = lastUpdated.HasValue ? lastUpdated.Value : DateTime.Now
            };

        public static SportEvent CreateHandicapSportEvent(
            string matchId,
            DateTime? lastUpdated = null,
            List<Book> books = null)
            => new SportEvent
            {
                id = matchId,
                markets = new List<Market>
                        {
                            new Market
                            {
                                odds_type_id = 5,
                                name = "asian_handicap",
                                group_name = "regular",
                                books = books ?? new List<Book>
                                {
                                    CreateHandicapBook("sr:book:4", "Unibet 4"),
                                    CreateHandicapBook("sr:book:5", "Unibet 5"),
                                    CreateHandicapBook("sr:book:6", "Unibet 6")
                                }
                            }
                        },
                markets_last_updated = lastUpdated.HasValue ? lastUpdated.Value : DateTime.Now
            };

        public static SportEvent CreateOverUnderSportEvent(
            string matchId,
            DateTime? lastUpdated = null,
            List<Book> books = null)
            => new SportEvent
            {
                id = matchId,
                markets = new List<Market>
                        {
                            new Market
                            {
                                odds_type_id = 3,
                                name = "total",
                                group_name = "regular",
                                books = books ?? new List<Book>
                                {
                                    CreateOverUnderBook("sr:book:7", "Unibet 7"),
                                    CreateOverUnderBook("sr:book:8", "Unibet 8"),
                                    CreateOverUnderBook("sr:book:9", "Unibet 9")
                                }
                            }
                        },
                markets_last_updated = lastUpdated.HasValue ? lastUpdated.Value : DateTime.Now
            };

        public static Book CreateOneXTwoBook(
            string id,
            string name,
            string homeOdds = "4.751",
            string awayOdds = "1.75",
            string drawOdds = "2.75")
            => new Book
            {
                id = id,
                name = name,
                outcomes = new List<Outcome>
                {
                    new Outcome { type = "home", odds = homeOdds, opening_odds = homeOdds },
                    new Outcome { type = "away", odds = awayOdds, opening_odds = awayOdds },
                    new Outcome { type = "draw", odds = drawOdds, opening_odds = drawOdds }
                }
            };

        public static Book CreateHandicapBook(
            string id,
            string name,
            string homeOdds = "1.80",
            string awayOdds = "1.95",
            float handicap = 0.25f)
            => new Book
            {
                id = id,
                name = name,
                outcomes = new List<Outcome>
                {
                    new Outcome { type = "home", odds = homeOdds, handicap = handicap.ToString(), opening_odds = homeOdds },
                    new Outcome { type = "away", odds = awayOdds, handicap = (-handicap).ToString(), opening_odds = awayOdds }
                }
            };

        public static Book CreateOverUnderBook(
            string id,
            string name,
            string homeOdds = "1.850",
            string awayOdds = "1.950",
            string total = "2")
            => new Book
            {
                id = id,
                name = name,
                outcomes = new List<Outcome>
                {
                    new Outcome { type = "over", odds = homeOdds, total = total, opening_odds = homeOdds, opening_total = double.Parse(total) },
                    new Outcome { type = "under", odds = awayOdds, total = total, opening_odds = awayOdds, opening_total = double.Parse(total) }
                }
            };
    }
}
namespace Soccer.DateProviders.SportRadar.Odds
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Newtonsoft.Json;
    using Soccer.DataProviders.SportRadar.Odds.DataMappers;
    using Soccer.DataProviders.SportRadar.Odds.Dtos;
    using Xunit;

    [Trait("Soccer.API", "Odds")]
    public class OddsMapperTests
    {
        [Fact]
        public void MapToMatchOddsList_DtoIsNull_ReturnEmptyList()
        {
            var matchOdds = OddsMapper.MapToMatchOddsList(null);

            Assert.Empty(matchOdds);
        }

        [Fact]
        public void MapToMatchOddsList_NoEvent_ReturnEmptyList()
        {
            var matchOdds = OddsMapper.MapToMatchOddsList(new OddsScheduleDto());

            Assert.Empty(matchOdds);
        }

        [Fact]
        public void MapToMatchOddsList_1X2BetType_ReturnListOddsByMatch()
        {
            var matchOdds = OddsMapper.MapToMatchOddsList(StubOddsScheduleDto());

            Assert.Equal(2, matchOdds.Count());
            Assert.Equal("match_id_1", matchOdds.ElementAt(0).MatchId);
            Assert.Equal("match_id_2", matchOdds.ElementAt(1).MatchId);
        }

        [Fact]
        public void BuildBetTypeOddsList_BetTypeIsNotSupport_ReturnEmptyOddsList()
        {
            var oddsList = OddsMapper.MapToMatchOdds(Stub1x2SportEvent("match_id_1", "4way")).BetTypeOddsList;

            Assert.Empty(oddsList);
        }

        [Fact]
        public void BuildBetTypeOddsList_1x2Data_ReturnOddsList()
        {
            var oddsList = OddsMapper.MapToMatchOdds(Stub1x2SportEvent("match_id_1", "3way")).BetTypeOddsList;
            var demo = JsonConvert.SerializeObject(oddsList);

            var firstBetTypeOdds = oddsList.FirstOrDefault();

            Assert.Equal(1, firstBetTypeOdds.Id);
            Assert.Equal("1x2", firstBetTypeOdds.Name);
            Assert.Equal("sr:book:1", firstBetTypeOdds.Bookmaker.Id);
            Assert.Equal("Unibet", firstBetTypeOdds.Bookmaker.Name);
            Assert.Equal(3, firstBetTypeOdds.BetOptions.Count());

            var firstBetOptionOdds = firstBetTypeOdds.BetOptions.FirstOrDefault();
            Assert.Equal(4.75m, firstBetOptionOdds.LiveOdds);
            Assert.Equal(3.96m, firstBetOptionOdds.OpeningOdds);
            Assert.Equal("home", firstBetOptionOdds.Type);
            Assert.Equal("away", firstBetTypeOdds.BetOptions.ElementAt(1).Type);
            Assert.Equal("draw", firstBetTypeOdds.BetOptions.ElementAt(2).Type);
        }

        [Fact]
        public void BuildBetTypeOddsList_AsianHandicapData_ReturnOddsList()
        {
            var oddsList = OddsMapper.MapToMatchOdds(StubHandicapSportEvent("match_id_1")).BetTypeOddsList;

            Assert.Equal(3, oddsList.Count());
            var firstBetTypeOdds = oddsList.FirstOrDefault();

            Assert.Equal(3, firstBetTypeOdds.Id);
            Assert.Equal("Asian Handicap", firstBetTypeOdds.Name);
            Assert.Equal("sr:book:4", firstBetTypeOdds.Bookmaker.Id);
            Assert.Equal("Unibet", firstBetTypeOdds.Bookmaker.Name);
            Assert.Equal(2, firstBetTypeOdds.BetOptions.Count());

            var firstBetOptionOdds = firstBetTypeOdds.BetOptions.FirstOrDefault();
            Assert.Equal("home", firstBetOptionOdds.Type);
            Assert.Equal(1.8m, firstBetOptionOdds.LiveOdds);
            Assert.Equal(1.8m, firstBetOptionOdds.OpeningOdds);
            Assert.Equal("-0.25", firstBetOptionOdds.OptionValue);

            var secondBetOptionOdds = firstBetTypeOdds.BetOptions.ElementAt(1);
            Assert.Equal("away", secondBetOptionOdds.Type);
            Assert.Equal(1.95m, secondBetOptionOdds.LiveOdds);
            Assert.Equal(1.95m, secondBetOptionOdds.OpeningOdds);
            Assert.Equal("0.25", secondBetOptionOdds.OptionValue);
        }

        [Fact]
        public void BuildBetTypeOddsList_OverUnderData_ReturnOddsList()
        {
            var oddsList = OddsMapper.MapToMatchOdds(StubOverUnderSportEvent("match_id_1")).BetTypeOddsList;

            Assert.Equal(3, oddsList.Count());
            var firstBetTypeOdds = oddsList.FirstOrDefault();

            Assert.Equal(2, firstBetTypeOdds.Id);
            Assert.Equal("OverUnder", firstBetTypeOdds.Name);
            Assert.Equal("sr:book:7", firstBetTypeOdds.Bookmaker.Id);
            Assert.Equal("Unibet", firstBetTypeOdds.Bookmaker.Name);
            Assert.Equal(2, firstBetTypeOdds.BetOptions.Count());

            var firstBetOptionOdds = firstBetTypeOdds.BetOptions.FirstOrDefault();
            Assert.Equal("over", firstBetOptionOdds.Type);
            Assert.Equal(1.85m, firstBetOptionOdds.LiveOdds);
            Assert.Equal(1.8m, firstBetOptionOdds.OpeningOdds);
            Assert.Equal("2", firstBetOptionOdds.OptionValue);
            Assert.Equal("2.5", firstBetOptionOdds.OpeningOptionValue);

            var secondBetOptionOdds = firstBetTypeOdds.BetOptions.ElementAt(1);
            Assert.Equal("under", secondBetOptionOdds.Type);
            Assert.Equal(1.95m, secondBetOptionOdds.LiveOdds);
            Assert.Equal(2m, secondBetOptionOdds.OpeningOdds);
            Assert.Equal("2", secondBetOptionOdds.OptionValue);
            Assert.Equal("2.5", secondBetOptionOdds.OpeningOptionValue);
        }

        private static OddsScheduleDto StubOddsScheduleDto()
            => new OddsScheduleDto
            {
                sport_events = new List<SportEvent>
                {
                    Stub1x2SportEvent("match_id_1", "3way"),
                    Stub1x2SportEvent("match_id_2", "3way")
                }
            };

        private static SportEvent Stub1x2SportEvent(string matchId, string onextwoBetTypeName)
            => new SportEvent
            {
                id = matchId,
                markets = new List<Market>
                        {
                            new Market
                            {
                                odds_type_id = 2,
                                name = onextwoBetTypeName,
                                group_name = "regular",
                                books = new List<Book>
                                {
                                    StubOneXTwoBook("sr:book:1", "Unibet"),
                                    StubOneXTwoBook("sr:book:2", "Unibet 2"),
                                    StubOneXTwoBook("sr:book:3", "Unibet 3")
                                }
                            }
                        }

            };

        private static SportEvent StubHandicapSportEvent(string matchId)
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
                                books = new List<Book>
                                {
                                    StubHandicapBook("sr:book:4", "Unibet"),
                                    StubHandicapBook("sr:book:5", "Unibet 2"),
                                    StubHandicapBook("sr:book:6", "Unibet 3")
                                }
                            }
                        }

            };

        private static SportEvent StubOverUnderSportEvent(string matchId)
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
                                books = new List<Book>
                                {
                                    StubOverUnderBook("sr:book:7", "Unibet"),
                                    StubOverUnderBook("sr:book:8", "Unibet 2"),
                                    StubOverUnderBook("sr:book:9", "Unibet 3")
                                }
                            }
                        }

            };

        private static Book StubOneXTwoBook(string id, string name)
            => new Book
            {
                id = id,
                name = name,
                outcomes = new List<Outcome>
                {
                    new Outcome { type = "home", odds = "4.751", opening_odds = "3.9555" },
                    new Outcome { type = "away", odds = "1.75", opening_odds = "2.950" },
                    new Outcome { type = "draw", odds = "2.75", opening_odds = "1.950" }
                }
            };

        private static Book StubHandicapBook(string id, string name)
            => new Book
            {
                id = id,
                name = name,
                outcomes = new List<Outcome>
                {
                    new Outcome { type = "home", odds = "1.80", handicap = "-0.25" },
                    new Outcome { type = "away", odds = "1.95", handicap = "0.25" }
                }
            };

        private static Book StubOverUnderBook(string id, string name)
            => new Book
            {
                id = id,
                name = name,
                outcomes = new List<Outcome>
                {
                    new Outcome { type = "over", odds = "1.850", total = "2", opening_odds = "1.800", opening_total = 2.5 },
                    new Outcome { type = "under", odds = "1.950", total = "2", opening_odds = "2.000", opening_total = 2.5 }
                }
            };
    }
}

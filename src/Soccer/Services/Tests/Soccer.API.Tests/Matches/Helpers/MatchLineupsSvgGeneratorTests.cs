using System;
using System.Collections.Generic;
using NSubstitute;
using Soccer.API.Matches.Helpers;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.Core.Teams.Models;
using Svg;
using Xunit;

namespace Soccer.API.Tests.Matches.Helpers
{
    [Trait("Soccer.API", "Match/Helpers")]
    public class MatchLineupsSvgGeneratorTests
    {
        private const string svgFolderPath = "svgFolderPath";
        private readonly Func<string, SvgDocument> getSvgDocumentFunc;
        private readonly MatchLineupsSvgGenerator matchLineupsSvgGenerator;

        public MatchLineupsSvgGeneratorTests()
        {
            getSvgDocumentFunc = Substitute.For<Func<string, SvgDocument>>();

            matchLineupsSvgGenerator = new MatchLineupsSvgGenerator(svgFolderPath, getSvgDocumentFunc);
        }

        [Fact]
        public void Generate_NoTeamInformation_ReturnEmpty()
        {
            var matchLineups = new MatchLineups();

            var lineupSvg = matchLineupsSvgGenerator.Generate(matchLineups);

            Assert.Empty(lineupSvg);
        }

        [Fact]
        public void Generate_NoDataHomeTeam_ReturnEmpty()
        {
            var matchLineups = new MatchLineups("11", DateTimeOffset.Now, null, StubAwayLineups());

            var lineupSvg = matchLineupsSvgGenerator.Generate(matchLineups);

            Assert.Empty(lineupSvg);
        }

        [Fact]
        public void Generate_NoDataAwayTeam_ReturnEmpty()
        {
            var matchLineups = new MatchLineups("11", DateTimeOffset.Now, StubHomeLineups(), null);

            var lineupSvg = matchLineupsSvgGenerator.Generate(matchLineups);

            Assert.Empty(lineupSvg);
        }

        [Fact]
        public void Generate_MatchLineupHasData_ReturnLineupSvg()
        {
            var matchLineups = StubMatchLineups(StubHomeLineups(), StubAwayLineups());

            getSvgDocumentFunc.Invoke("svgFolderPath/stadium.svg").Returns(StubStadiumDocument());
            getSvgDocumentFunc.Invoke("svgFolderPath/jersey.svg").Returns(StubJerseyDocument());
            getSvgDocumentFunc.Invoke($"svgFolderPath/{EventType.ScoreChange.DisplayName}.svg").Returns(StubEventTypeDocument());
            getSvgDocumentFunc.Invoke($"svgFolderPath/{EventType.ScoreChangeByOwnGoal.DisplayName}.svg").Returns(StubEventTypeDocument());
            getSvgDocumentFunc.Invoke($"svgFolderPath/{EventType.ScoreChangeByPenalty.DisplayName}.svg").Returns(StubEventTypeDocument());
            getSvgDocumentFunc.Invoke($"svgFolderPath/{EventType.Substitution.DisplayName}.svg").Returns(StubEventTypeDocument());

            var actualSvg = matchLineupsSvgGenerator.Generate(matchLineups);

            var expectedSvg = "<svg xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" xmlns:xml=\"http://www.w3.org/XML/1998/namespace\" version=\"1.1\">" +
                            "  <circle r=\"12\" transform=\"translate(178,24)\" style=\"fill:#30C2FF;\" />" +
                            "  <text x=\"178\" y=\"28\" text-anchor=\"middle\" font-family=\"Roboto\" font-size=\"11\" font-weight=\"400\" style=\"fill:white;\">1</text>" +
                            "  <text x=\"178\" y=\"50\" text-anchor=\"middle\" font-family=\"Roboto\" font-size=\"11\" font-weight=\"400\" style=\"fill:white;\">player1 name</text>" +
                            "  <rect width=\"8\" height=\"12\" rx=\"1\" transform=\"translate(186,7)\" filter=\"url(#shadow)\" style=\"fill:yellow;\" />" +
                            "  <g transform=\"translate(144.8,5.4)\">" +
                            "    <path />" +
                            "  </g>" +
                            "  <g transform=\"translate(158.8,0)\">" +
                            "    <path />" +
                            "  </g>" +
                            "  <g transform=\"translate(158.8,26)\">" +
                            "    <path />" +
                            "  </g>" +
                            "  <g transform=\"translate(182,26)\">" +
                            "    <path />" +
                            "  </g>" +
                            "  <circle r=\"12\" transform=\"translate(178,111)\" style=\"fill:#30C2FF;\" />" +
                            "  <text x=\"178\" y=\"115\" text-anchor=\"middle\" font-family=\"Roboto\" font-size=\"11\" font-weight=\"400\" style=\"fill:white;\">2</text>" +
                            "  <text x=\"178\" y=\"137\" text-anchor=\"middle\" font-family=\"Roboto\" font-size=\"11\" font-weight=\"400\" style=\"fill:white;\">player2 name</text>" +
                            "  <g transform=\"translate(186,94)\" filter=\"url(#shadow)\">" +
                            "    <rect width=\"8\" height=\"12\" rx=\"1\" style=\"fill:yellow;\" />" +
                            "    <rect x=\"2\" y=\"-2\" width=\"8\" height=\"12\" rx=\"1\" style=\"fill:red;\" />" +
                            "  </g>" +
                            "  <g>" +
                            "    <g transform=\"translate(144.8,92.4)\">" +
                            "      <path />" +
                            "    </g>" +
                            "    <circle cx=\"147.8\" cy=\"97.4\" r=\"5\" style=\"fill:red;\" />" +
                            "    <text x=\"147.8\" y=\"99.4\" text-anchor=\"middle\" font-family=\"Roboto\" font-size=\"7\" font-weight=\"700\" style=\"fill:white;\">2</text>" +
                            "  </g>" +
                            "  <g transform=\"translate(158.8,87)\">" +
                            "    <path />" +
                            "  </g>" +
                            "  <g>" +
                            "    <g transform=\"translate(158.8,113)\">" +
                            "      <path />" +
                            "    </g>" +
                            "    <circle cx=\"161.8\" cy=\"117\" r=\"5\" style=\"fill:red;\" />" +
                            "    <text x=\"161.8\" y=\"119\" text-anchor=\"middle\" font-family=\"Roboto\" font-size=\"7\" font-weight=\"700\" style=\"fill:white;\">2</text>" +
                            "  </g>" +
                            "  <g transform=\"translate(182,113)\">" +
                            "    <path />" +
                            "  </g>" +
                            "  <circle r=\"12\" transform=\"translate(178,198)\" style=\"fill:#30C2FF;\" />" +
                            "  <text x=\"178\" y=\"202\" text-anchor=\"middle\" font-family=\"Roboto\" font-size=\"11\" font-weight=\"400\" style=\"fill:white;\">3</text>" +
                            "  <text x=\"178\" y=\"224\" text-anchor=\"middle\" font-family=\"Roboto\" font-size=\"11\" font-weight=\"400\" style=\"fill:white;\">player3 name</text>" +
                            "  <rect width=\"8\" height=\"12\" rx=\"1\" transform=\"translate(186,181)\" filter=\"url(#shadow)\" style=\"fill:red;\" />" +
                            "  <g transform=\"translate(182,200)\">" +
                            "    <path />" +
                            "  </g>" +
                            "  <circle r=\"12\" transform=\"translate(178,24)\" style=\"fill:#30C2FF;\" />" +
                            "  <text x=\"178\" y=\"28\" text-anchor=\"middle\" font-family=\"Roboto\" font-size=\"11\" font-weight=\"400\" style=\"fill:white;\">4</text>" +
                            "  <text x=\"178\" y=\"50\" text-anchor=\"middle\" font-family=\"Roboto\" font-size=\"11\" font-weight=\"400\" style=\"fill:white;\">player4 name</text>" +
                            "  <rect width=\"8\" height=\"12\" rx=\"1\" transform=\"translate(186,7)\" filter=\"url(#shadow)\" style=\"fill:yellow;\" />" +
                            "  <g transform=\"translate(144.8,5.4)\">" +
                            "    <path />" +
                            "  </g>" +
                            "  <g transform=\"translate(158.8,0)\">" +
                            "    <path />" +
                            "  </g>" +
                            "  <g transform=\"translate(158.8,26)\">" +
                            "    <path />" +
                            "  </g>" +
                            "  <g transform=\"translate(182,26)\">" +
                            "    <path />" +
                            "  </g>" +
                            "  <circle r=\"12\" transform=\"translate(119,155)\" style=\"fill:#30C2FF;\" />" +
                            "  <text x=\"119\" y=\"159\" text-anchor=\"middle\" font-family=\"Roboto\" font-size=\"11\" font-weight=\"400\" style=\"fill:white;\">5</text>" +
                            "  <text x=\"119\" y=\"181\" text-anchor=\"middle\" font-family=\"Roboto\" font-size=\"11\" font-weight=\"400\" style=\"fill:white;\">player5 name</text>" +
                            "  <rect width=\"8\" height=\"12\" rx=\"1\" transform=\"translate(127,138)\" filter=\"url(#shadow)\" style=\"fill:red;\" />" +
                            "  <g transform=\"translate(123,157)\">" +
                            "    <path />" +
                            "  </g>" +
                            "  <circle r=\"12\" transform=\"translate(238,155)\" style=\"fill:#30C2FF;\" />" +
                            "  <text x=\"238\" y=\"159\" text-anchor=\"middle\" font-family=\"Roboto\" font-size=\"11\" font-weight=\"400\" style=\"fill:white;\">6</text>" +
                            "  <text x=\"238\" y=\"181\" text-anchor=\"middle\" font-family=\"Roboto\" font-size=\"11\" font-weight=\"400\" style=\"fill:white;\">player6 name</text>" +
                            "</svg>";


            Assert.Equal(expectedSvg, actualSvg.Replace("\r\n", string.Empty));
        }

        private MatchLineups StubMatchLineups(TeamLineups homeTeam = null, TeamLineups awayTeam = null)
            => new MatchLineups(
                "matchId",
                new DateTimeOffset(new DateTime(1989, 5, 28)),
                homeTeam ?? StubHomeLineups(),
                awayTeam ?? StubAwayLineups());

        private TeamLineups StubHomeLineups()
            => new TeamLineups(
                "homeid",
                "homename",
                true,
                new Coach("coachid", "coachname", "national", "country code"),
                "1-1",
                new List<Player>
                {
                    new Player("player1", "player1 name", PlayerType.Goalkeeper, 1, Position.Goalkeeper, 1)
                    {
                        EventStatistic = new Dictionary<EventType, int>
                        {
                            { EventType.ScoreChange, 1 },
                            { EventType.ScoreChangeByOwnGoal, 1 },
                            { EventType.ScoreChangeByPenalty, 1 },
                            { EventType.YellowCard, 1 },
                            { EventType.Substitution, 1 }
                        }
                    },
                    new Player("player2", "player2 name", PlayerType.Midfielder, 2, Position.CentralMidfielder, 2)
                    {
                        EventStatistic = new Dictionary<EventType, int>
                        {
                            { EventType.ScoreChange, 2 },
                            { EventType.ScoreChangeByOwnGoal, 2 },
                            { EventType.ScoreChangeByPenalty, 1 },
                            { EventType.YellowRedCard, 1 },
                            { EventType.Substitution, 1 }
                        }
                    },
                    new Player("player3", "player3 name", PlayerType.Forward, 3, Position.Striker, 3)
                    {
                        EventStatistic = new Dictionary<EventType, int>
                        {
                            { EventType.RedCard, 1 },
                            { EventType.Substitution, 1 }
                        }
                    }
                },
                new List<Player>());

        private TeamLineups StubAwayLineups()
            => new TeamLineups(
                "awayid",
                "awayname",
                true,
                new Coach("coachid", "coachname", "national", "country code"),
                "2-0",
                new List<Player>
                {
                    new Player("player4", "player4 name", PlayerType.Goalkeeper, 4, Position.Goalkeeper, 1)
                    {
                        EventStatistic = new Dictionary<EventType, int>
                        {
                            { EventType.ScoreChange, 1 },
                            { EventType.ScoreChangeByOwnGoal, 1 },
                            { EventType.ScoreChangeByPenalty, 1 },
                            { EventType.YellowCard, 1 },
                            { EventType.Substitution, 1 }
                        }
                    },
                    new Player("player5", "player5 name", PlayerType.Defender, 5, Position.LeftBack, 2)
                    {
                        EventStatistic = new Dictionary<EventType, int>
                        {
                            { EventType.RedCard, 1 },
                            { EventType.Substitution, 1 }
                        }
                    },
                    new Player("player6", "player6 name", PlayerType.Forward, 6, Position.RightWinger, 3)
                },
                new List<Player>());

        private static SvgDocument StubJerseyDocument()
        {
            var document = new SvgDocument();
            var jerseyElement = new SvgPath();
            document.Children.Add(jerseyElement);

            return document;
        }

        private static SvgDocument StubStadiumDocument()
            => new SvgDocument();

        private static SvgElement StubEventTypeDocument()
        {
            var document = new SvgDocument();
            var element = new SvgPath();
            document.Children.Add(element);

            return document;
        }
    }
}
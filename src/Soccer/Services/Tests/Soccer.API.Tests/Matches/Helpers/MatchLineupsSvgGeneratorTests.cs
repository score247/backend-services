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
                             "  <path pathLength=\"0\" transform=\"translate(162,10)\" style=\"fill:#30C2FF;\" />" +
                             "  <text x=\"178\" y=\"28\" text-anchor=\"middle\" font-family=\"Roboto\" font-size=\"11\" font-weight=\"400\" style=\"fill:white;\">1</text>" +
                             "  <text x=\"178\" y=\"48\" text-anchor=\"middle\" font-family=\"Roboto\" font-size=\"11\" font-weight=\"400\" style=\"fill:white;\">player1 name</text>" +
                             "  <rect width=\"8\" height=\"12\" rx=\"1\" transform=\"translate(183.33333333333334,5)\" style=\"fill:yellow;\" />" +
                             "  <path transform=\"translate(144,3)\" />" +
                             "  <path transform=\"translate(159,-2.5)\" />" +
                             "  <path transform=\"translate(159,24.4)\" />" +
                             "  <g transform=\"translate(180,26)\">" +
                             "    <path />" +
                             "  </g>" +
                             "  <path pathLength=\"0\" transform=\"translate(162,97)\" style=\"fill:#30C2FF;\" />" +
                             "  <text x=\"178\" y=\"115\" text-anchor=\"middle\" font-family=\"Roboto\" font-size=\"11\" font-weight=\"400\" style=\"fill:white;\">2</text>" +
                             "  <text x=\"178\" y=\"135\" text-anchor=\"middle\" font-family=\"Roboto\" font-size=\"11\" font-weight=\"400\" style=\"fill:white;\">player2 name</text>" +
                             "  <g transform=\"translate(183.33333333333334,92)\">" +
                             "    <rect width=\"8\" height=\"12\" rx=\"1\" style=\"fill:yellow;\" />" +
                             "    <rect x=\"2\" y=\"-2\" width=\"8\" height=\"12\" rx=\"1\" style=\"fill:red;\" />" +
                             "  </g>" +
                             "  <g>" +
                             "    <path transform=\"translate(144,3)\" />" +
                             "    <circle cx=\"147\" cy=\"95\" r=\"4\" style=\"fill:red;\" />" +
                             "    <text x=\"147\" y=\"97\" text-anchor=\"middle\" font-family=\"Roboto\" font-size=\"6\" font-weight=\"400\" style=\"fill:white;\">2</text>" +
                             "  </g>" +
                             "  <path transform=\"translate(159,-2.5)\" />" +
                             "  <g>" +
                             "    <path transform=\"translate(159,24.4)\" />" +
                             "    <circle cx=\"162\" cy=\"115.4\" r=\"4\" style=\"fill:red;\" />" +
                             "    <text x=\"162\" y=\"117.4\" text-anchor=\"middle\" font-family=\"Roboto\" font-size=\"6\" font-weight=\"400\" style=\"fill:white;\">2</text>" +
                             "  </g>" +
                             "  <g transform=\"translate(180,113)\">" +
                             "    <path />" +
                             "  </g>" +
                             "  <path pathLength=\"0\" transform=\"translate(162,184)\" style=\"fill:#30C2FF;\" />" +
                             "  <text x=\"178\" y=\"202\" text-anchor=\"middle\" font-family=\"Roboto\" font-size=\"11\" font-weight=\"400\" style=\"fill:white;\">3</text>" +
                             "  <text x=\"178\" y=\"222\" text-anchor=\"middle\" font-family=\"Roboto\" font-size=\"11\" font-weight=\"400\" style=\"fill:white;\">player3 name</text>" +
                             "  <rect width=\"8\" height=\"12\" rx=\"1\" transform=\"translate(183.33333333333334,179)\" style=\"fill:red;\" />" +
                             "  <g transform=\"translate(180,200)\">" +
                             "    <path />" +
                             "  </g>" +
                             "  <path pathLength=\"0\" transform=\"translate(162,10)\" style=\"fill:#30C2FF;\" />" +
                             "  <text x=\"178\" y=\"28\" text-anchor=\"middle\" font-family=\"Roboto\" font-size=\"11\" font-weight=\"400\" style=\"fill:white;\">4</text>" +
                             "  <text x=\"178\" y=\"48\" text-anchor=\"middle\" font-family=\"Roboto\" font-size=\"11\" font-weight=\"400\" style=\"fill:white;\">player4 name</text>" +
                             "  <rect width=\"8\" height=\"12\" rx=\"1\" transform=\"translate(183.33333333333334,5)\" style=\"fill:yellow;\" />" +
                             "  <path transform=\"translate(144,3)\" />" +
                             "  <path transform=\"translate(159,-2.5)\" />" +
                             "  <path transform=\"translate(159,24.4)\" />" +
                             "  <g transform=\"translate(180,26)\">" +
                             "    <path />" +
                             "  </g>" +
                             "  <path pathLength=\"0\" transform=\"translate(103,141)\" style=\"fill:#30C2FF;\" />" +
                             "  <text x=\"119\" y=\"159\" text-anchor=\"middle\" font-family=\"Roboto\" font-size=\"11\" font-weight=\"400\" style=\"fill:white;\">5</text>" +
                             "  <text x=\"119\" y=\"179\" text-anchor=\"middle\" font-family=\"Roboto\" font-size=\"11\" font-weight=\"400\" style=\"fill:white;\">player5 name</text>" +
                             "  <rect width=\"8\" height=\"12\" rx=\"1\" transform=\"translate(124.33333333333333,136)\" style=\"fill:red;\" />" +
                             "  <g transform=\"translate(121,157)\">" +
                             "    <path />" +
                             "  </g>" +
                             "  <path pathLength=\"0\" transform=\"translate(222,141)\" style=\"fill:#30C2FF;\" />" +
                             "  <text x=\"238\" y=\"159\" text-anchor=\"middle\" font-family=\"Roboto\" font-size=\"11\" font-weight=\"400\" style=\"fill:white;\">6</text>" +
                             "  <text x=\"238\" y=\"179\" text-anchor=\"middle\" font-family=\"Roboto\" font-size=\"11\" font-weight=\"400\" style=\"fill:white;\">player6 name</text>" +
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

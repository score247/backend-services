using System;
using System.Collections.Generic;
using System.Text;
using NSubstitute;
using Soccer.API.Matches.Helpers;
using Soccer.Core.Matches.Models;
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
        public void Generate_HomeFormationIsNull_ReturnEmpty()
        {
            var matchLineups = new MatchLineups();

            var lineupSvg = matchLineupsSvgGenerator.Generate(matchLineups);

            Assert.Empty(lineupSvg);
        }

        [Fact]
        public void Generate_AwayFormationIsNull_ReturnEmpty()
        {
            var matchLineups = new MatchLineups();

            var lineupSvg = matchLineupsSvgGenerator.Generate(matchLineups);

            Assert.Empty(lineupSvg);
        }

        [Fact]
        public void Generate_MatchLineupHasData_ReturnLineupSvg()
        {
            var homeLineups = StubHomeLineups();
            var awayLineups = StubAwayLineups();


        }

        //private MatchLineups StubMatchLineups()
        //    => new MatchLineups(
        //        "matchId", 
        //        new DateTimeOffset(new DateTime(1989, 5, 28)),
        //        new TeamLineups("team1", "team home", true, new Coach()))

        private TeamLineups StubHomeLineups()
            => new TeamLineups(
                "homeid",
                "homename",
                true,
                new Coach("coachid", "coachname", "national", "country code"),
                "1-1",
                new List<Player>
                {
                    new Player("player1", "player1 name", PlayerType.Goalkeeper, 1, Position.Goalkeeper, 1),
                    new Player("player2", "player2 name", PlayerType.Midfielder, 2, Position.CentralMidfielder, 2),
                    new Player("player3", "player3 name", PlayerType.Forward, 3, Position.Striker, 3)
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
                    new Player("player4", "player4 name", PlayerType.Goalkeeper, 4, Position.Goalkeeper, 1),
                    new Player("player5", "player5 name", PlayerType.Defender, 5, Position.LeftBack, 2),
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
        {
            var document = new SvgDocument();
            var jerseyElement = new SvgPath();
            document.Children.Add(jerseyElement);

            return document;
        }
    }
}

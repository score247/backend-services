using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Soccer.Core.Matches.Models;
using Soccer.Core.Teams.Models;
using Svg;
using Svg.Transforms;

namespace Soccer.API.Matches.Helpers
{
    public interface IMatchLineupsGenerator
    {
        string Generate(MatchLineups matchLineups);
    }

    public class MatchLineupsSvgGenerator : IMatchLineupsGenerator
    {
        private const int stadiumWidth = 357;
        private const int stadiumHeight = 526;
        private const string svgText = "<svg";
        private readonly string svgFolderPath;
        private readonly Func<string, SvgDocument> getSvgDocumentFunc;

        public MatchLineupsSvgGenerator(
            string svgFolderPath,
            Func<string, SvgDocument> getSvgDocumentFunc)
        {
            this.svgFolderPath = svgFolderPath;
            this.getSvgDocumentFunc = getSvgDocumentFunc;
        }

        public string Generate(MatchLineups matchLineups)
        {
            if (string.IsNullOrWhiteSpace(matchLineups?.Home?.Formation)
                || string.IsNullOrWhiteSpace(matchLineups?.Away?.Formation))
            {
                return string.Empty;
            }

            var lineupsDoc = getSvgDocumentFunc($"{svgFolderPath}/stadium.svg");
            var jerseyElement = GetDefaultJerseyElement();

            var lineupsElements = RenderTeam(matchLineups.Home, jerseyElement);
            lineupsElements = lineupsElements.Concat(RenderTeam(matchLineups.Away, jerseyElement));

            foreach (var element in lineupsElements)
            {
                lineupsDoc.Children.Add(element);
            }

            lineupsDoc.FlushStyles(true);

            var lineupsSvg = lineupsDoc.GetXML();

            return lineupsSvg.Substring(lineupsSvg.IndexOf(svgText, StringComparison.OrdinalIgnoreCase));
        }

        private SvgPath GetDefaultJerseyElement()
        {
            var jerseyDoc = getSvgDocumentFunc($"{svgFolderPath}/jersey.svg");
            var jerseyElement = jerseyDoc.Children.FirstOrDefault() as SvgPath;

            return jerseyElement;
        }

        private static IEnumerable<SvgElement> RenderTeam(
            TeamLineups teamLineups, 
            SvgPath jeyseyElement)
        {
            var players = new List<SvgElement>();
            var formationSplits = teamLineups.ConvertFormationToList();
            var totalRow = formationSplits.Count;
            var rowIndex = 0;
            var playerIndex = 0;
            var yPlayerDistance = stadiumHeight / 2 / totalRow;

            foreach (var formationItem in formationSplits)
            {
                var rowPlayers = RenderPlayerRow(
                    jeyseyElement, 
                    teamLineups.Players.Skip(playerIndex).Take(formationItem), 
                    teamLineups.IsHome, 
                    rowIndex++, 
                    yPlayerDistance);

                players = players.Concat(rowPlayers).ToList();
                playerIndex += formationItem;
            }

            return players;
        }

        private static List<SvgElement> RenderPlayerRow(
            SvgPath jeyseyElement, 
            IEnumerable<Player> players, 
            bool isHome,
            int rowIndex,
            int yPlayerDistance)
        {
            var playerElements = new List<SvgElement>();
            var totalPlayer = players.Count();
            var playerDistance = stadiumWidth / (totalPlayer + 1);

            for (int playerIndex = 0; playerIndex < totalPlayer; playerIndex++)
            {
                var x = playerDistance * (playerIndex + 1) - 16;
                var heightGap = rowIndex * yPlayerDistance + (isHome ? 10 : 50);
                var y = isHome ? heightGap : (stadiumHeight - heightGap);

                playerElements.Add(RenderPlayerJersey(jeyseyElement, isHome, x, y));

                var player = players.ElementAt(playerIndex);
                playerElements.Add(RenderPlayerNumber(player, x, y));
                playerElements.Add(RenderPlayerName(player, x, y));
            }

            return playerElements;
        }

        private static SvgElement RenderPlayerJersey(SvgPath jeyseyElement, bool isHome, int x, int y)
        {
            var playerElement = jeyseyElement.DeepCopy();
            playerElement.CustomAttributes.Add("transform", $"translate({x},{y})");
            // home color: #30C2FF
            // away color: #FA2E58
            playerElement.AddStyle("fill", isHome ? "#30C2FF" : "#FA2E58", 0);
            return playerElement;
        }

        private static SvgText RenderPlayerNumber(Player player, int x, int y)
        {
            var playerNumber = new SvgText
            {
                Text = player.JerseyNumber.ToString(),
                TextAnchor = SvgTextAnchor.Middle,
                X = new SvgUnitCollection { new SvgUnit(x + 15.5f) },
                Y = new SvgUnitCollection { new SvgUnit(y + 18) },
                FontFamily = "Roboto",
                FontWeight = SvgFontWeight.Bold,
                FontSize = 12
            };

            playerNumber.AddStyle("fill", "white", 0);

            return playerNumber;
        }

        private static SvgText RenderPlayerName(Player player, int x, int y)
        {
            var playerName = new SvgText
            {
                Text = player.Name,
                TextAnchor = SvgTextAnchor.Middle,
                X = new SvgUnitCollection { new SvgUnit(x + 15.5f) },
                Y = new SvgUnitCollection { new SvgUnit(y + 38) },
                FontFamily = "Roboto",
                FontWeight = SvgFontWeight.Bold,
                FontSize = 12
            };

            playerName.AddStyle("fill", "white", 0);

            return playerName;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.Core.Teams.Models;
using Svg;

#pragma warning disable S109 // Magic numbers should not be used

namespace Soccer.API.Matches.Helpers
{
    public interface IMatchLineupsGenerator
    {
        string Generate(MatchLineups matchLineups);
    }

    public class MatchLineupsSvgGenerator : IMatchLineupsGenerator
    {
        private const int playerWidth = 32;
        private const int playerHeight = 36;
        private const int stadiumWidth = 357;
        private const int stadiumHeight = 526;
        private const string svgText = "<svg";
        private const string homeColor = "#30C2FF";
        private const string awayColor = "#FA2E58";
        private const string robotoFontName = "Roboto";
        private const string fillStyleName = "fill";
        private const string whiteColor = "#fff";
        private const int fontSize = 11;
        private const string tranformAttributeName = "transform";
        private readonly string svgFolderPath;
        private readonly Func<string, SvgDocument> getSvgDocumentFunc;

        private readonly IList<Func<Player, int, int, SvgElement>> statisticRenderers;

        public MatchLineupsSvgGenerator(
            string svgFolderPath,
            Func<string, SvgDocument> getSvgDocumentFunc)
        {
            this.svgFolderPath = svgFolderPath;
            this.getSvgDocumentFunc = getSvgDocumentFunc;
            statisticRenderers = new List<Func<Player, int, int, SvgElement>>
            {
                RenderCards,
                RenderGoals,
                RenderPenaltyGoals,
                RenderOwnGoals,
                RenderPlayerOut
            };
        }

        public string Generate(MatchLineups matchLineups)
        {
            if (string.IsNullOrWhiteSpace(matchLineups?.Home?.Formation)
                || string.IsNullOrWhiteSpace(matchLineups?.Away?.Formation))
            {
                return string.Empty;
            }

            var jerseyElement = GetDefaultJerseyElement();

            var lineupsElements =
                RenderTeam(matchLineups.Home, jerseyElement)
                .Concat(RenderTeam(matchLineups.Away, jerseyElement));

            return BuildLineupSvg(lineupsElements);
        }

        private string BuildLineupSvg(IEnumerable<SvgElement> lineupsElements)
        {
            var lineupsDoc = getSvgDocumentFunc($"{svgFolderPath}/stadium.svg");

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

        private IEnumerable<SvgElement> RenderTeam(
            TeamLineups teamLineups,
            SvgPath jeyseyElement)
        {
            var players = new List<SvgElement>();
            var formationSplits = teamLineups.ConvertFormationToList();
            var totalRow = formationSplits.Count(fm => fm > 0);
            var teamPlayers = teamLineups.Players.OrderBy(pl => pl.Order);
            var rowIndex = 0;
            var playerIndex = 0;
            var yPlayerDistance = stadiumHeight / 2 / totalRow;

            foreach (var formationItem in formationSplits)
            {
                var formationRowPlayers = teamPlayers.Skip(playerIndex).Take(formationItem);
                var rowPlayers = RenderPlayerRow(
                    jeyseyElement,
                    teamLineups.IsHome ? formationRowPlayers : formationRowPlayers.Reverse(),
                    teamLineups.IsHome,
                    rowIndex++,
                    yPlayerDistance);

                players = players.Concat(rowPlayers).ToList();
                playerIndex += formationItem;
            }

            return players;
        }

        private List<SvgElement> RenderPlayerRow(
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
                var x = playerDistance * (playerIndex + 1) - (playerWidth / 2);
                var heightGap = rowIndex * yPlayerDistance + (isHome ? 10 : 50);
                var y = isHome ? heightGap : (stadiumHeight - heightGap);

                playerElements.Add(RenderPlayerJersey(jeyseyElement, isHome, x, y));

                var player = players.ElementAt(playerIndex);
                playerElements.Add(RenderPlayerNumber(player, x, y));
                playerElements.Add(RenderPlayerName(player, x, y));
                playerElements.AddRange(RenderStatisticElements(player, x, y));
            }

            return playerElements;
        }

        private List<SvgElement> RenderStatisticElements(Player player, int x, int y)
        {
            var elements = new List<SvgElement>();

            if (player.EventStatistic != null
                && player.EventStatistic.Any())
            {
                foreach (var renderer in statisticRenderers)
                {
                    var element = renderer(player, x, y);

                    if (element != null)
                    {
                        elements.Add(element);
                    }
                }
            }

            return elements;
        }

        private static SvgElement RenderPlayerJersey(SvgPath jeyseyElement, bool isHome, int x, int y)
        {
            var playerElement = jeyseyElement.DeepCopy();
            playerElement.CustomAttributes.Add(tranformAttributeName, $"translate({x},{y})");
            playerElement.AddStyle(fillStyleName, isHome ? homeColor : awayColor, 0);
            return playerElement;
        }

        private static SvgText RenderPlayerNumber(Player player, int x, int y)
        {
            var playerNumber = new SvgText
            {
                Text = player.JerseyNumber.ToString(),
                TextAnchor = SvgTextAnchor.Middle,
                X = new SvgUnitCollection { new SvgUnit(x + (playerWidth / 2)) },
                Y = new SvgUnitCollection { new SvgUnit(y + (playerHeight / 2)) },
                FontFamily = robotoFontName,
                FontWeight = SvgFontWeight.Normal,
                FontSize = fontSize
            };

            playerNumber.AddStyle(fillStyleName, whiteColor, 0);

            return playerNumber;
        }

        private static SvgText RenderPlayerName(Player player, int x, int y)
        {
            var playerName = new SvgText
            {
                Text = player.Name,
                TextAnchor = SvgTextAnchor.Middle,
                X = new SvgUnitCollection { new SvgUnit(x + (playerWidth / 2)) },
                Y = new SvgUnitCollection { new SvgUnit(y + playerHeight + 2) },
                FontFamily = robotoFontName,
                FontWeight = SvgFontWeight.Normal,
                FontSize = fontSize
            };

            playerName.AddStyle(fillStyleName, whiteColor, 0);

            return playerName;
        }

        private SvgElement RenderCards(Player player, int x, int y)
        {
            var hasRedcard = player.EventStatistic.ContainsKey(EventType.RedCard);
            var hasYellowCard = player.EventStatistic.ContainsKey(EventType.YellowCard);
            var hasYellowRedCard = player.EventStatistic.ContainsKey(EventType.YellowRedCard);

            if (hasRedcard)
            {
                var redCardElement = BuildCardElement(Color.Red);

                redCardElement.CustomAttributes.Add(tranformAttributeName, $"translate({x + playerWidth / 1.5},{y - 5})");

                return redCardElement;
            }

            if (hasYellowRedCard)
            {
                var redCardElement = BuildYellowRedCardElement();
                redCardElement.CustomAttributes.Add(tranformAttributeName, $"translate({x + playerWidth / 1.5},{y - 5})");
                return redCardElement;
            }

            if (hasYellowCard)
            {
                var yellowCardElement = BuildCardElement(Color.Yellow);
                yellowCardElement.CustomAttributes.Add(tranformAttributeName, $"translate({x + playerWidth / 1.5},{y - 5})");
                return yellowCardElement;
            }

            return default;
        }

        private SvgElement RenderGoals(Player player, int x, int y)
        {
            var hasGoals = player.EventStatistic.ContainsKey(EventType.ScoreChange);

            if (hasGoals)
            {
                var hasPenaltyGoal = player.EventStatistic.ContainsKey(EventType.ScoreChangeByPenalty);

                var element = BuildEventTypeElement(EventType.ScoreChange);
                var elementX = x - 3 + (hasPenaltyGoal ? -15 : 0);
                var elementY = y - 7;
                if (!element.CustomAttributes.ContainsKey(tranformAttributeName))
                {
                    element.CustomAttributes.Add(tranformAttributeName, $"translate({elementX},{elementY})");
                }

                var totalGoals = player.EventStatistic[EventType.ScoreChange];
                if (totalGoals > 1)
                {
                    return BuildEventCountNumber(elementX + 3, (float)(elementY + 7), element, totalGoals);
                }

                return element;
            }

            return default;
        }

        private SvgElement BuildEventCountNumber(float x, float y, SvgElement goalElement, int totalGoals)
        {
            var goalNumberElements = BuildNumberEventElement(totalGoals, x, y - 2);
            var groupElement = new SvgGroup();
            groupElement.Children.Add(goalElement);
            foreach (var element in goalNumberElements)
            {
                groupElement.Children.Add(element);
            }

            return groupElement;
        }

        private IEnumerable<SvgElement> BuildNumberEventElement(int total, float x, float y)
        {
            var groupElement = new List<SvgElement>();

            var circleElement = new SvgCircle
            {
                CenterX = x,
                CenterY = y,
                Radius = 4
            };
            circleElement.AddStyle("fill", "red", 0);

            groupElement.Add(circleElement);

            var playerNumber = new SvgText
            {
                Text = total.ToString(),
                TextAnchor = SvgTextAnchor.Middle,
                X = new SvgUnitCollection { new SvgUnit(x) },
                Y = new SvgUnitCollection { new SvgUnit(y + 2) },
                FontFamily = robotoFontName,
                FontWeight = SvgFontWeight.Normal,
                FontSize = 6
            };
            playerNumber.AddStyle("fill", "white", 0);

            groupElement.Add(playerNumber);

            return groupElement;
        }

        private SvgElement RenderPenaltyGoals(Player player, int x, int y)
        {
            var hasGoals = player.EventStatistic.ContainsKey(EventType.ScoreChangeByPenalty);

            if (hasGoals)
            {
                var element = BuildEventTypeElement(EventType.ScoreChangeByPenalty);
                var elementX = x - 3;
                var elementY = y - 12.5;
                if (!element.CustomAttributes.ContainsKey(tranformAttributeName))
                {
                    element.CustomAttributes.Add(tranformAttributeName, $"translate({elementX},{elementY})");
                }

                var totalGoals = player.EventStatistic[EventType.ScoreChangeByPenalty];
                if (totalGoals > 1)
                {
                    return BuildEventCountNumber(elementX + 3, (float)(elementY + 12.5), element, totalGoals);
                }

                return element;
            }

            return default;
        }

        private SvgElement RenderOwnGoals(Player player, int x, int y)
        {
            var hasGoals = player.EventStatistic.ContainsKey(EventType.ScoreChangeByOwnGoal);

            if (hasGoals)
            {
                var element = BuildEventTypeElement(EventType.ScoreChangeByOwnGoal);
                var elementX = x - 3;
                var elementY = y + playerHeight / 2.5;
                if (!element.CustomAttributes.ContainsKey(tranformAttributeName))
                {
                    element.CustomAttributes.Add(tranformAttributeName, $"translate({elementX},{elementY})");
                }

                var totalGoals = player.EventStatistic[EventType.ScoreChangeByOwnGoal];
                if (totalGoals > 1)
                {
                    return BuildEventCountNumber(elementX + 3, (float)(elementY + 6), element, totalGoals);
                }

                return element;
            }

            return default;
        }

        private SvgElement RenderPlayerOut(Player player, int x, int y)
        {
            var hasSubtitution = player.EventStatistic.ContainsKey(EventType.Substitution);

            if (hasSubtitution)
            {
                var element = BuildSubtituteOutElement();
                element.CustomAttributes.Add(tranformAttributeName, $"translate({x + 18},{y + 16})");

                return element;
            }

            return default;
        }

        private SvgRectangle BuildCardElement(Color color)
            => new SvgRectangle
            {
                Width = 8,
                Height = 12,
                CornerRadiusX = 1,
                Fill = new SvgColourServer(color)
            };

        private SvgElement BuildYellowRedCardElement()
        {
            var group = new SvgGroup();

            group.Children.Add(new SvgRectangle
            {
                Width = 8,
                Height = 12,
                CornerRadiusX = 1,
                Fill = new SvgColourServer(Color.Yellow)
            });

            group.Children.Add(new SvgRectangle
            {
                Width = 8,
                Height = 12,
                CornerRadiusX = 1,
                X = 2,
                Y = -2,
                Fill = new SvgColourServer(Color.Red)
            });

            return group;
        }

        private SvgElement BuildEventTypeElement(EventType eventType)
        {
            var document = getSvgDocumentFunc($"{svgFolderPath}/{eventType.DisplayName}.svg");
            var element = document.Children.FirstOrDefault() as SvgPath;

            return element;
        }

        private SvgElement BuildSubtituteOutElement()
        {
            var document = getSvgDocumentFunc($"{svgFolderPath}/substitution.svg");
            var group = new SvgGroup();

            foreach (var child in document.Children)
            {
                group.Children.Add(child);
            }

            return group;
        }
    }
}

#pragma warning restore S109 // Magic numbers should not be used
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
        private const int playerWidth = 24;
        private const int playerHeight = 24;
        private const int stadiumWidth = 357;
        private const int stadiumHeight = 526;
        private const int ballWidthHeight = 16;
        private const int totalEventRadius = 5;
        private const int cardWidth = 8;
        private const int cardHeight = 12;

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

            var lineupsElements = RenderTeam(matchLineups.Home).Concat(RenderTeam(matchLineups.Away));

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

        private IEnumerable<SvgElement> RenderTeam(TeamLineups teamLineups)
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
                var x = playerDistance * (playerIndex + 1);
                var heightGap = rowIndex * yPlayerDistance + playerHeight;
                var y = isHome ? heightGap : (stadiumHeight - heightGap);

                playerElements.Add(RenderPlayerJersey(isHome, x, y));

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

        private static SvgElement RenderPlayerJersey(bool isHome, int x, int y)
        {
            var playerElement = new SvgCircle
            {
                Radius = playerHeight / 2
            };
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
                X = new SvgUnitCollection { new SvgUnit(x) },
                Y = new SvgUnitCollection { new SvgUnit(y + 4) },
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
                X = new SvgUnitCollection { new SvgUnit(x) },
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
            SvgElement card = null;
            var cardX = x + playerWidth / 3;
            var cardY = y - playerWidth / 1.5 - 1;

            if (hasRedcard)
            {
                card = BuildCardElement(Color.Red);
                card.CustomAttributes.Add(tranformAttributeName, $"translate({cardX},{cardY})");
            }

            if (hasYellowRedCard)
            {
                card = BuildYellowRedCardElement();
                card.CustomAttributes.Add(tranformAttributeName, $"translate({cardX},{cardY})");
            }

            if (hasYellowCard)
            {
                card = BuildCardElement(Color.Yellow);
                card.CustomAttributes.Add(tranformAttributeName, $"translate({cardX},{cardY})");
            }

            if(card != null)
            {
                card.CustomAttributes.Add("filter", $"url(#shadow)");
            }

            return card;
        }

        private SvgElement RenderGoals(Player player, int x, int y)
        {
            var hasGoals = player.EventStatistic.ContainsKey(EventType.ScoreChange);

            if (hasGoals)
            {
                var hasPenaltyGoal = player.EventStatistic.ContainsKey(EventType.ScoreChangeByPenalty);
                var overTwoPenaltyGoals = hasPenaltyGoal && player.EventStatistic[EventType.ScoreChangeByPenalty] > 1
                    ? 0
                    : 3;

                var element = BuildEventTypeElement(EventType.ScoreChange);
                var elementX = x - (playerWidth / 1.25) + (hasPenaltyGoal ? -(ballWidthHeight + 1 - overTwoPenaltyGoals) : 0);
                var elementY = (float)y - (playerHeight / 2) - (ballWidthHeight / 2) + 1.4;
                if (!element.CustomAttributes.ContainsKey(tranformAttributeName))
                {
                    element.CustomAttributes.Add(tranformAttributeName, $"translate({elementX},{elementY})");
                }

                var totalGoals = player.EventStatistic[EventType.ScoreChange];
                if (totalGoals > 1)
                {
                    return BuildEventCountNumber((float)(elementX + 3), (float)(elementY + 7), element, totalGoals);
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
                Radius = totalEventRadius
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
                FontWeight = SvgFontWeight.Bold,
                FontSize = 7
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
                var elementX = x - playerWidth / 1.25;
                var elementY = y - playerHeight;
                if (!element.CustomAttributes.ContainsKey(tranformAttributeName))
                {
                    element.CustomAttributes.Add(tranformAttributeName, $"translate({elementX},{elementY})");
                }

                var totalGoals = player.EventStatistic[EventType.ScoreChangeByPenalty];
                if (totalGoals > 1)
                {
                    return BuildEventCountNumber((float)(elementX + 3), (float)(elementY + 12.5), element, totalGoals);
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
                var elementX = x - playerWidth / 1.25;
                var elementY = y + 2;
                if (!element.CustomAttributes.ContainsKey(tranformAttributeName))
                {
                    element.CustomAttributes.Add(tranformAttributeName, $"translate({elementX},{elementY})");
                }

                var totalGoals = player.EventStatistic[EventType.ScoreChangeByOwnGoal];
                if (totalGoals > 1)
                {
                    return BuildEventCountNumber((float)(elementX + 3), (float)(elementY + 6), element, totalGoals);
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
                var subX = x + (ballWidthHeight / 4);
                var subY = y + 2;

                var element = BuildSubtituteOutElement();
                element.CustomAttributes.Add(tranformAttributeName, $"translate({subX},{subY})");

                return element;
            }

            return default;
        }

        private SvgElement BuildCardElement(Color color)
            => new SvgRectangle
            {
                Width = cardWidth,
                Height = cardHeight,
                CornerRadiusX = 1,
                Fill = new SvgColourServer(color)
            };

        private SvgElement BuildYellowRedCardElement()
        {
            var group = new SvgGroup();

            group.Children.Add(new SvgRectangle
            {
                Width = cardWidth,
                Height = cardHeight,
                CornerRadiusX = 1,
                Fill = new SvgColourServer(Color.Yellow)
            });

            group.Children.Add(new SvgRectangle
            {
                 Width = cardWidth,
                Height = cardHeight,
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

            return LoadGroupChildElement(document);
        }

        private SvgElement BuildSubtituteOutElement()
        {
            var document = getSvgDocumentFunc($"{svgFolderPath}/substitution.svg");

            return LoadGroupChildElement(document);
        }

        private static SvgElement LoadGroupChildElement(SvgDocument document)
        {
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
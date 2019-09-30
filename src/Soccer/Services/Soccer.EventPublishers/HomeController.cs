namespace Soccer.EventProcessors
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SignalR;
    using Newtonsoft.Json;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Odds.Models;
    using Soccer.Core.Odds.SignalREvents;
    using Soccer.Core.Shared.Enumerations;
    using Soccer.Core.Teams.Models;
    using Soccer.EventPublishers.Hubs;
    using Soccer.EventPublishers.Matches.SignalR;
    using Soccer.EventPublishers.Odds.SignalR;
    using Soccer.EventPublishers.Teams.SignalR;

    public class HomeController : ControllerBase
    {
        private readonly IHubContext<SoccerHub> hubContext;

        public HomeController(IHubContext<SoccerHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        public IActionResult Index()
        {
            var clients = JsonConvert.SerializeObject(SoccerHub.GetAllActiveConnections(), Formatting.Indented);

            return new ContentResult
            {
                Content = $"Clients Info: {clients}",
                ContentType = "text/html"
            };
        }

        public async Task<IActionResult> MockPushEvents()
        {
            await MockPushMatchEvent();
            await MockPushOddsComparisonEvent();
            await MockPushOddsMovementEvent();
            await MockPushTeamStatisticEvent();

            var clients = JsonConvert.SerializeObject(SoccerHub.GetAllActiveConnections(), Formatting.Indented);

            return new ContentResult
            {
                Content = $"Sent to All Client: {clients}"
            };
        }

        private async Task MockPushMatchEvent()
        {
            var matchEventMessage = new MatchEventSignalRMessage(
                Sport.Soccer.Value,
                new MatchEvent("leagueId", "matchId", new MatchResult
                {
                    EventStatus = MatchStatus.Closed,
                    HomeScore = 1,
                    AwayScore = 1
                },
                new TimelineEvent
                {
                    Id = "121212112",
                    MatchClock = "11:11"
                }));

            await hubContext.Clients.All.SendAsync("MatchEvent", JsonConvert.SerializeObject(matchEventMessage));
        }

        private async Task MockPushOddsComparisonEvent()
        {
            var oddsComparisonMessage = new OddsComparisonSignalRMessage(
                    Sport.Soccer.Value,
                    "1111",
                    new List<BetTypeOdds>
                    {
                        new BetTypeOdds(
                            1,
                            "1x2",
                            new Bookmaker("1", "Bookmarker"),
                            DateTime.Now,
                            new List<BetOptionOdds>
                            {
                                new BetOptionOdds("Home", 1m, 1m, "1", "3")
                            })
                    });

            await hubContext.Clients.All.SendAsync("OddsComparison", JsonConvert.SerializeObject(oddsComparisonMessage));
        }

        private async Task MockPushOddsMovementEvent()
        {
            var oddsMovementMessage = new OddsMovementSignalRMessage(
                    Sport.Soccer.Value,
                    "1111",
                    new List<OddsMovementEvent>
                    {
                        new OddsMovementEvent(
                            1,
                            new Bookmaker("1", "Bookmarker"),
                            new OddsMovement(new List<BetOptionOdds>
                            {
                                new BetOptionOdds("home", 1.1m, 1.2m, "12112", "cvcvc")
                            },
                            "Live",
                            DateTimeOffset.Now))
                    });

            await hubContext.Clients.All.SendAsync("OddsMovement", JsonConvert.SerializeObject(oddsMovementMessage));
        }

        private async Task MockPushTeamStatisticEvent()
        {
            var teamStatisticMessage = new TeamStatisticSignalRMessage(
                    Sport.Soccer.Value,
                    "1111",
                    true,
                    new TeamStatistic
                    {
                        RedCards = 2
                    });

            await hubContext.Clients.All.SendAsync("TeamStatistic", JsonConvert.SerializeObject(teamStatisticMessage));
        }
    }
}
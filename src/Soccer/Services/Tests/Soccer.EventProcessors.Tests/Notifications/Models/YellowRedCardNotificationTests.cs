using AutoFixture;
using NSubstitute;
using Score247.Shared.Tests;
using Soccer.Core._Shared.Resources;
using Soccer.Core.Matches.Models;
using Soccer.Core.Teams.Models;
using Soccer.EventProcessors.Notifications.Models;
using Xunit;

namespace Soccer.EventProcessors.Tests.Notifications.Models
{
    public class YellowRedCardNotificationTests
    {
        private readonly ILanguageResourcesService resources;
        private readonly Fixture fixture;

        public YellowRedCardNotificationTests()
        {
            resources = Substitute.For<ILanguageResourcesService>();
            resources
                .GetString(Arg.Is<string>(name => name == "YellowRedCard"))
                .Returns("Yellow Red card");
            resources
                .GetString(Arg.Is<string>(name => name == "YellowRedCardPostfix"))
                .Returns("2nd yellow card and is off!");

            resources
                .GetString(Arg.Is<string>(name => name == "PlayerToBeDefined"))
                .Returns("(Player TBD)");

            fixture = new Fixture();
        }

        [Fact]
        public void Title_ShouldReturnCorrectFormat()
        {
            var scoreEvent = fixture.Create<TimelineEvent>();
            var stoppageTimeDisplay = string.IsNullOrWhiteSpace(scoreEvent?.StoppageTime)
                ? string.Empty
                : $"+{scoreEvent?.StoppageTime}";

            var redCard = new YellowRedCardNotification(
                            resources,
                            scoreEvent,
                            fixture.Create<Team>(),
                            fixture.Create<Team>(),
                            scoreEvent.MatchTime,
                            fixture.Create<MatchResult>());

            Assert.Equal($"Yellow Red card ({scoreEvent.MatchTime}{stoppageTimeDisplay}')", redCard.Title());
        }

        [Fact]
        public void Content_HomeReceivedButMissingPlayer_ShouldReturnCorrectFormat()
        {
            var homeTeam = fixture.Create<Team>();
            var awayTeam = fixture.Create<Team>();
            var redCardEvent = fixture.Create<TimelineEvent>()
                .With(timeline => timeline.Team, "home")
                .With(timeline => timeline.Player, null);

            var redCard = new YellowRedCardNotification(
                            resources,
                            redCardEvent,
                            homeTeam,
                            awayTeam,
                            redCardEvent.MatchTime,
                            fixture.Create<MatchResult>());

            Assert.Equal($"{homeTeam.Name} - (Player TBD) 2nd yellow card and is off!", redCard.Content());
        }

        [Fact]
        public void Content_HomeReceived_ShouldReturnCorrectFormat()
        {
            var homeTeam = fixture.Create<Team>();
            var awayTeam = fixture.Create<Team>();
            var redCardEvent = fixture.Create<TimelineEvent>()
                .With(timeline => timeline.Team, "home")
                .With(timeline => timeline.Player, fixture.Create<Player>());

            var redCard = new YellowRedCardNotification(
                            resources,
                            redCardEvent,
                            homeTeam,
                            awayTeam,
                            redCardEvent.MatchTime,
                            fixture.Create<MatchResult>());

            Assert.Equal($"{homeTeam.Name} - {redCardEvent.Player.Name} 2nd yellow card and is off!", redCard.Content());
        }

        [Fact]
        public void Content_AwayReceivedButMissingPlayer_ShouldReturnCorrectFormat()
        {
            var homeTeam = fixture.Create<Team>();
            var awayTeam = fixture.Create<Team>();
            var redCardEvent = fixture.Create<TimelineEvent>()
                .With(timeline => timeline.Team, "away")
                .With(timeline => timeline.Player, null);

            var redCard = new YellowRedCardNotification(
                            resources,
                            redCardEvent,
                            homeTeam,
                            awayTeam,
                            redCardEvent.MatchTime,
                            fixture.Create<MatchResult>());

            Assert.Equal($"{awayTeam.Name} - (Player TBD) 2nd yellow card and is off!", redCard.Content());
        }

        [Fact]
        public void Content_AwayReceived_ShouldReturnCorrectFormat()
        {
            var homeTeam = fixture.Create<Team>();
            var awayTeam = fixture.Create<Team>();
            var redCardEvent = fixture.Create<TimelineEvent>()
                .With(timeline => timeline.Team, "away")
                .With(timeline => timeline.Player, fixture.Create<Player>());

            var redCard = new YellowRedCardNotification(
                            resources,
                            redCardEvent,
                            homeTeam,
                            awayTeam,
                            redCardEvent.MatchTime,
                            fixture.Create<MatchResult>());

            Assert.Equal($"{awayTeam.Name} - {redCardEvent.Player.Name} 2nd yellow card and is off!", redCard.Content());
        }
    }
}

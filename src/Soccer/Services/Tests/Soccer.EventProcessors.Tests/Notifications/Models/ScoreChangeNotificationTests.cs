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
    public class ScoreChangeNotificationTests
    {
        private readonly ILanguageResourcesService resources;
        private readonly Fixture fixture;

        public ScoreChangeNotificationTests()
        {
            resources = Substitute.For<ILanguageResourcesService>();
            resources
                .GetString(Arg.Is<string>(name => name == "NotificationScoreChange"))
                .Returns("GOAL! {0}");

            fixture = new Fixture();
        }

        [Fact]
        public void Title_ShouldReturnCorrectFormat()
        {
            var scoreEvent = fixture.Create<TimelineEvent>();
            var stoppageTimeDisplay = string.IsNullOrWhiteSpace(scoreEvent?.StoppageTime)
                ? string.Empty
                : $"+{scoreEvent?.StoppageTime}";

            var scoreChanged = new ScoreChangeNotification(
                            resources,
                            scoreEvent,
                            fixture.Create<Team>(),
                            fixture.Create<Team>(),
                            scoreEvent.MatchTime,
                            fixture.Create<MatchResult>());

            Assert.Equal($"GOAL! ({scoreEvent.MatchTime}{stoppageTimeDisplay}')", scoreChanged.Title());
        }

        [Fact]
        public void Content_HomeScored_ShouldReturnCorrectFormat()
        {
            var homeTeam = fixture.Create<Team>();
            var awayTeam = fixture.Create<Team>();
            var scoreEvent = fixture.Create<TimelineEvent>()
                .With(timeline => timeline.Team, "home");
            var result = fixture.Create<MatchResult>();

            var scoreChanged = new ScoreChangeNotification(
                            resources,
                            scoreEvent,
                            homeTeam,
                            awayTeam,
                            scoreEvent.MatchTime,
                            result);

            Assert.Equal($"{homeTeam.Name} [{result.HomeScore}] - {result.AwayScore} {awayTeam.Name}", scoreChanged.Content());
        }

        [Fact]
        public void Content_AwayScored_ShouldReturnCorrectFormat()
        {
            var homeTeam = fixture.Create<Team>();
            var awayTeam = fixture.Create<Team>();
            var scoreEvent = fixture.Create<TimelineEvent>()
                .With(timeline => timeline.Team, "away");
            var result = fixture.Create<MatchResult>();

            var scoreChanged = new ScoreChangeNotification(
                            resources,
                            scoreEvent,
                            homeTeam,
                            awayTeam,
                            scoreEvent.MatchTime,
                            result);

            Assert.Equal($"{homeTeam.Name} {result.HomeScore} - [{result.AwayScore}] {awayTeam.Name}", scoreChanged.Content());
        }
    }
}

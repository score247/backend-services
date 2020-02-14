using AutoFixture;
using NSubstitute;
using Soccer.Core._Shared.Resources;
using Soccer.Core.Matches.Models;
using Soccer.Core.Teams.Models;
using Soccer.EventProcessors.Notifications.Models;
using Xunit;

namespace Soccer.EventProcessors.Tests.Notifications.Models
{
    public class MatchStartNotificationTests
    {
        private readonly ILanguageResourcesService resources;
        private readonly Fixture fixture;

        public MatchStartNotificationTests() 
        {
            resources = Substitute.For<ILanguageResourcesService>();
            resources
                .GetString(Arg.Is<string>(name => name == "MatchStarted"))
                .Returns("Match started");

            fixture = new Fixture();
        }

        [Fact]
        public void Title_ShouldReturnCorrectFormat() 
        {
            var matchStarted = new MatchStartedNotification(
                resources,
                fixture.Create<TimelineEvent>(),
                fixture.Create<Team>(),
                fixture.Create<Team>(),
                0,
                fixture.Create<MatchResult>());

            Assert.Equal("Match started", matchStarted.Title());
        }

        [Fact]
        public void Content_ShouldReturnCorrectFormat()
        {
            var homeTeam = fixture.Create<Team>();
            var awayTeam = fixture.Create<Team>();
            var matchStarted = new MatchStartedNotification(
                resources,
                fixture.Create<TimelineEvent>(),
                homeTeam,
                awayTeam,
                0,
                fixture.Create<MatchResult>());

            Assert.Equal($"{homeTeam.Name} 0 - 0 {awayTeam.Name}", matchStarted.Content());
        }
    }
}

using System.Collections.Generic;
using AutoFixture;
using NSubstitute;
using Score247.Shared.Tests;
using Soccer.Core._Shared.Resources;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.Core.Teams.Models;
using Soccer.EventProcessors.Notifications.Models;
using Xunit;

namespace Soccer.EventProcessors.Tests.Notifications.Models
{
    public class MatchEndNotificationTests
    {
        private readonly ILanguageResourcesService resources;
        private readonly Fixture fixture;

        public MatchEndNotificationTests()
        {
            resources = Substitute.For<ILanguageResourcesService>();
            SetLanguageResources();

            fixture = new Fixture();
        }

        [Fact]
        public void Title_NormalMatch_ShouldReturnCorrectFormat()
        {
            var timelineEvent = fixture.Create<TimelineEvent>();
            var matchResult = fixture.Create<MatchResult>()
                .With(result => result.EventStatus, MatchStatus.Ended)
                .With(result => result.MatchPeriods, fixture.CreateMany<MatchPeriod>(2));

            var matchEnd = new MatchEndNotification(
                            resources,
                            timelineEvent,
                            fixture.Create<Team>(),
                            fixture.Create<Team>(),
                            timelineEvent.MatchTime,
                            matchResult);

            Assert.Equal($"Match ended", matchEnd.Title());
        }

        [Fact]
        public void Content_NormalMatchAndNotSecondLeg_ShouldReturnCorrectFormat()
        {
            var timelineEvent = fixture.Create<TimelineEvent>();
            var matchResult = fixture.Create<MatchResult>()
                .With(result => result.EventStatus, MatchStatus.Ended)
                .With(result => result.MatchPeriods, fixture.CreateMany<MatchPeriod>(2))
                .With(result => result.AggregateWinnerId, null);
            var homeTeam = fixture.Create<Team>();
            var awayTeam = fixture.Create<Team>();

            var matchEnd = new MatchEndNotification(
                            resources,
                            timelineEvent,
                            homeTeam,
                            awayTeam,
                            timelineEvent.MatchTime,
                            matchResult);

            Assert.Equal($"{homeTeam.Name} {matchResult.HomeScore} - {matchResult.AwayScore} {awayTeam.Name}", matchEnd.Content());
        }

        [Fact]
        public void Content_NormalMatchAndSecondLeg_AggWinnerIsHome_ShouldReturnCorrectFormat()
        {
            var homeTeam = fixture.Create<Team>();
            var awayTeam = fixture.Create<Team>();
            var timelineEvent = fixture.Create<TimelineEvent>();
            var matchResult = fixture.Create<MatchResult>()
                .With(result => result.EventStatus, MatchStatus.Ended)
                .With(result => result.MatchPeriods, fixture.CreateMany<MatchPeriod>(2))
                .With(result => result.AggregateWinnerId, homeTeam.Id);
           

            var expectedContent = $"{homeTeam.Name} {matchResult.HomeScore} - {matchResult.AwayScore} {awayTeam.Name}";
            expectedContent += $"\nAgg: {matchResult.AggregateHomeScore} - {matchResult.AggregateAwayScore}. {homeTeam.Name} Win";

            var matchEnd = new MatchEndNotification(
                            resources,
                            timelineEvent,
                            homeTeam,
                            awayTeam,
                            timelineEvent.MatchTime,
                            matchResult);

            Assert.Equal(expectedContent, matchEnd.Content());
        }

        [Fact]
        public void Content_NormalMatchAndSecondLeg_AggWinnerIsAway_ShouldReturnCorrectFormat()
        {
            var homeTeam = fixture.Create<Team>();
            var awayTeam = fixture.Create<Team>();
            var timelineEvent = fixture.Create<TimelineEvent>();
            var matchResult = fixture.Create<MatchResult>()
                .With(result => result.EventStatus, MatchStatus.Ended)
                .With(result => result.MatchPeriods, fixture.CreateMany<MatchPeriod>(2))
                .With(result => result.AggregateWinnerId, awayTeam.Id);


            var expectedContent = $"{homeTeam.Name} {matchResult.HomeScore} - {matchResult.AwayScore} {awayTeam.Name}";
            expectedContent += $"\nAgg: {matchResult.AggregateHomeScore} - {matchResult.AggregateAwayScore}. {awayTeam.Name} Win";

            var matchEnd = new MatchEndNotification(
                            resources,
                            timelineEvent,
                            homeTeam,
                            awayTeam,
                            timelineEvent.MatchTime,
                            matchResult);

            Assert.Equal(expectedContent, matchEnd.Content());
        }

        [Fact]
        public void Title_MatchHasExtra_ShouldReturnCorrectFormat()
        {
            var matchPeriods = new List<MatchPeriod>
            {   
                fixture.Create<MatchPeriod>(),
                fixture.Create<MatchPeriod>(),
                fixture.Create<MatchPeriod>().With(period => period.PeriodType, PeriodType.Overtime)
            };

            var timelineEvent = fixture.Create<TimelineEvent>();
            var matchResult = fixture.Create<MatchResult>()
                .With(result => result.EventStatus, MatchStatus.Ended)
                .With(result => result.MatchPeriods, matchPeriods);

            var matchEnd = new MatchEndNotification(
                            resources,
                            timelineEvent,
                            fixture.Create<Team>(),
                            fixture.Create<Team>(),
                            timelineEvent.MatchTime,
                            matchResult);

            Assert.Equal($"Match ended after extra time", matchEnd.Title());
        }

        [Fact]
        public void Title_MatchHasPenaltyShootout_ShouldReturnCorrectFormat()
        {
            var matchPeriods = new List<MatchPeriod>
            {
                fixture.Create<MatchPeriod>(),
                fixture.Create<MatchPeriod>(),
                fixture.Create<MatchPeriod>().With(period => period.PeriodType, PeriodType.Penalties)
            };

            var timelineEvent = fixture.Create<TimelineEvent>();
            var matchResult = fixture.Create<MatchResult>()
                .With(result => result.EventStatus, MatchStatus.Ended)
                .With(result => result.MatchPeriods, matchPeriods);

            var matchEnd = new MatchEndNotification(
                            resources,
                            timelineEvent,
                            fixture.Create<Team>(),
                            fixture.Create<Team>(),
                            timelineEvent.MatchTime,
                            matchResult);

            Assert.Equal($"Match ended after penalty shoot-out", matchEnd.Title());
        }

        [Fact]
        public void Content_MatchHasPenaltyShootout_ShouldReturnCorrectFormat()
        {
            var homeTeam = fixture.Create<Team>();
            var awayTeam = fixture.Create<Team>();
            var penaltyPeriod = fixture.Create<MatchPeriod>().With(period => period.PeriodType, PeriodType.Penalties);
            var matchPeriods = new List<MatchPeriod>
            {
                fixture.Create<MatchPeriod>(),
                fixture.Create<MatchPeriod>(),
                penaltyPeriod
            };

            var timelineEvent = fixture.Create<TimelineEvent>();
            var matchResult = fixture.Create<MatchResult>()
                .With(result => result.EventStatus, MatchStatus.Ended)
                .With(result => result.MatchPeriods, matchPeriods)
                .With(result => result.AggregateWinnerId, null);

            var expectedContent = $"{homeTeam.Name} {matchResult.HomeScore} - {matchResult.AwayScore} {awayTeam.Name}";
            expectedContent += $"\nPenalty shoot-out: {homeTeam.Name} {penaltyPeriod.HomeScore} - {penaltyPeriod.AwayScore} {awayTeam.Name}";

            var matchEnd = new MatchEndNotification(
                            resources,
                            timelineEvent,
                            homeTeam,
                            awayTeam,
                            timelineEvent.MatchTime,
                            matchResult);

            Assert.Equal(expectedContent, matchEnd.Content());
        }

        [Fact]
        public void Content_MatchHasPenaltyShootout_AggWinnerIsHome_ShouldReturnCorrectFormat()
        {
            var homeTeam = fixture.Create<Team>();
            var awayTeam = fixture.Create<Team>();
            var penaltyPeriod = fixture.Create<MatchPeriod>().With(period => period.PeriodType, PeriodType.Penalties);
            var matchPeriods = new List<MatchPeriod>
            {
                fixture.Create<MatchPeriod>(),
                fixture.Create<MatchPeriod>(),
                penaltyPeriod
            };

            var timelineEvent = fixture.Create<TimelineEvent>();
            var matchResult = fixture.Create<MatchResult>()
                .With(result => result.EventStatus, MatchStatus.Ended)
                .With(result => result.MatchPeriods, matchPeriods)
                .With(result => result.AggregateWinnerId, homeTeam.Id);

            var expectedContent = $"{homeTeam.Name} {matchResult.HomeScore} - {matchResult.AwayScore} {awayTeam.Name}";
            expectedContent += $"\nPenalty shoot-out: {homeTeam.Name} {penaltyPeriod.HomeScore} - {penaltyPeriod.AwayScore} {awayTeam.Name}";
            expectedContent += $"\nAgg: {matchResult.AggregateHomeScore} - {matchResult.AggregateAwayScore}. {homeTeam.Name} Win";

            var matchEnd = new MatchEndNotification(
                            resources,
                            timelineEvent,
                            homeTeam,
                            awayTeam,
                            timelineEvent.MatchTime,
                            matchResult);

            Assert.Equal(expectedContent, matchEnd.Content());
        }

        private void SetLanguageResources()
        {
            resources
                .GetString(Arg.Is<string>(name => name == "NotificationMatchEnd"))
                .Returns("Match ended {0}");
            resources
                .GetString(Arg.Is<string>(name => name == "NotificationMatchEndAggregate"))
                .Returns("Agg: {0} - {1}. {2} Win");

            resources
                .GetString(Arg.Is<string>(name => name == "NotificationMatchEndPenalty"))
                .Returns("Penalty shoot-out: {0} {1} - {2} {3}");

            resources
                .GetString(Arg.Is<string>(name => name == "NotificationAfterExtraTime"))
                .Returns("after extra time");

            resources
                .GetString(Arg.Is<string>(name => name == "NotificationAfterPenalty"))
                .Returns("after penalty shoot-out");
        }
    }
}

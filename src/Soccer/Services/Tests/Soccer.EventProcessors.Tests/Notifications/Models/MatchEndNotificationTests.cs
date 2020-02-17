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
            expectedContent += $"\nPenalty shoot-out: {penaltyPeriod.HomeScore} - {penaltyPeriod.AwayScore}";

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
            expectedContent += $"\nAgg: {matchResult.AggregateHomeScore} - {matchResult.AggregateAwayScore}";
            expectedContent += $"\nPenalty shoot-out: {penaltyPeriod.HomeScore} - {penaltyPeriod.AwayScore}. {homeTeam.Name} Win";

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
                .GetString(Arg.Is<string>(name => name == "MatchEnd"))
                .Returns("Match ended");
            resources
                .GetString(Arg.Is<string>(name => name == "Aggregate"))
                .Returns("Agg:");

            resources
                .GetString(Arg.Is<string>(name => name == "PenaltyShootout"))
                .Returns("Penalty shoot-out:");

            resources
                .GetString(Arg.Is<string>(name => name == "AfterExtraTime"))
                .Returns("after extra time");

            resources
                .GetString(Arg.Is<string>(name => name == "AfterPenalty"))
                .Returns("after penalty shoot-out");

            resources
                .GetString(Arg.Is<string>(name => name == "Win"))
                .Returns("Win");
        }
    }
}

using AutoFixture;
using Score247.Shared.Tests;
using Soccer.Core.Matches.Extensions;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Xunit;

namespace Soccer.Core.Tests.Matches.Extensions
{
    [Trait("Soccer.EventProcessors", "FetchedLiveMatchConsumer")]
    public class TimelineExtensionsTests
    {
        private static readonly Fixture fixture = new Fixture();

        [Fact]
        public void ShouldReprocessScore_BreakStart_ReturnTrue()
        {
            var timeline = fixture
                .For<TimelineEvent>()
                .With(t => t.Type, EventType.BreakStart)
                .Create();

            Assert.True(timeline.ShouldReprocessScore());
        }

        [Fact]
        public void ShouldReprocessScore_MatchEnd_ReturnTrue()
        {
            var timeline = fixture
                .For<TimelineEvent>()
                .With(t => t.Type, EventType.MatchEnded)
                .Create();

            Assert.True(timeline.ShouldReprocessScore());
        }

        [Fact]
        public void ShouldReprocessScore_PenaltyMissed_ReturnTrue()
        {
            var timeline = fixture
                .For<TimelineEvent>()
                .With(t => t.Type, EventType.PenaltyMissed)
                .Create();

            Assert.True(timeline.ShouldReprocessScore());
        }

        [Fact]
        public void ShouldReprocessScore_ScoreChanged_ReturnFalse()
        {
            var timeline = fixture
                .For<TimelineEvent>()
                .With(t => t.Type, EventType.ScoreChange)
                .Create();

            Assert.False(timeline.ShouldReprocessScore());
        }

        [Fact]
        public void IsScoreChangeInPenalty_PenaltyPeriodButNotScored_ReturnFalse()
        {
            var timeline = fixture
                .For<TimelineEvent>()
                .With(t => t.Type, EventType.PenaltyMissed)
                .Create();

            Assert.False(timeline.IsScoreChangeInPenalty());
        }

        [Fact]
        public void IsScoreChangeInPenalty_RegularPeriod_ReturnFalse()
        {
            var timeline = fixture
                .For<TimelineEvent>()
                .With(t => t.Type, EventType.ScoreChange)
                .With(t => t.PeriodType, PeriodType.RegularPeriod)
                .Create();

            Assert.False(timeline.IsScoreChangeInPenalty());
        }

        [Fact]
        public void IsScoreChangeInPenalty_PenaltyPeriodAndScored_ReturnTrue()
        {
            var timeline = fixture
                .For<TimelineEvent>()
                .With(t => t.PeriodType, PeriodType.Penalties)
                .With(t => t.Type, EventType.ScoreChange)
                .Create();

            Assert.True(timeline.IsScoreChangeInPenalty());
        }
    }
}
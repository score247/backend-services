using FakeItEasy;
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
        [Fact]
        public void ShouldReprocessScore_BreakStart_ReturnTrue()
        {
            var timeline = A.Dummy<TimelineEvent>()
                .With(t => t.Type, EventType.BreakStart);
            Assert.True(timeline.ShouldReprocessScore());
        }

        [Fact]
        public void ShouldReprocessScore_MatchEnd_ReturnTrue()
        {
            var timeline = A.Dummy<TimelineEvent>()
                .With(t => t.Type, EventType.MatchEnded);
            Assert.True(timeline.ShouldReprocessScore());
        }

        [Fact]
        public void ShouldReprocessScore_PenaltyMissed_ReturnTrue()
        {
            var timeline = A.Dummy<TimelineEvent>()
                .With(t => t.Type, EventType.PenaltyMissed);
            Assert.True(timeline.ShouldReprocessScore());
        }

        [Fact]
        public void ShouldReprocessScore_ScoreChanged_ReturnFalse()
        {
            var timeline = A.Dummy<TimelineEvent>()
                .With(t => t.Type, EventType.ScoreChange);
            Assert.False(timeline.ShouldReprocessScore());
        }

        [Fact]
        public void IsScoreChangeInPenalty_PenaltyPeriodButNotScored_ReturnFalse()
        {
            var timeline = A.Dummy<TimelineEvent>()
                .With(t => t.Type, EventType.PenaltyMissed);
            Assert.False(timeline.IsScoreChangeInPenalty());
        }

        [Fact]
        public void IsScoreChangeInPenalty_RegularPeriod_ReturnFalse()
        {
            var timeline = A.Dummy<TimelineEvent>()
                .With(t => t.PeriodType, PeriodType.RegularPeriod)
                .With(t => t.Type, EventType.ScoreChange);
            Assert.False(timeline.IsScoreChangeInPenalty());
        }

        [Fact]
        public void IsScoreChangeInPenalty_PenaltyPeriodAndScored_ReturnTrue()
        {
            var timeline = A.Dummy<TimelineEvent>()
                .With(t => t.Type, EventType.ScoreChange)
                .With(t => t.PeriodType, PeriodType.Penalties);
            Assert.True(timeline.IsScoreChangeInPenalty());
        }
    }
}
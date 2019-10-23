using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Score247.Shared.Tests;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.EventProcessors.Matches.Filters;
using Xunit;

namespace Soccer.EventProcessors.Tests.Matches.Filters
{
    [Trait("Soccer.EventProcessors", "EventDateFilter")]
    public class EventDateFilterTests
    {
        private static readonly Fixture fixture = new Fixture();
        private readonly ILiveMatchFilter liveMatchFilter;

        public EventDateFilterTests()
        {
            liveMatchFilter = new LiveMatchFilter(new LiveMatchRangeValidator());
        }

        [Fact]
        public void FilterAsync_EmptyList_ShouldReturnEmpty()
        {
            var matches = new List<Match>();

            var filteredMatches = liveMatchFilter.FilterNotStarted(matches);

            Assert.Empty(filteredMatches);
        }

        [Fact]
        public void FilterAsync_NotHaveNotStartedMatches_ShouldReturnCorrectList()
        {
            var matches = new List<Match>
            {
                fixture.For<Match>()
                    .With(m => m.Id, "match:1")
                    .With(m => m.MatchResult, fixture.For<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.Closed)
                        .Create())
                    .With(m => m.LatestTimeline,  fixture.For<TimelineEvent>()
                            .With(t => t.Type, EventType.MatchEnded)
                            .With(t => t.Time, DateTimeOffset.Now)
                            .Create())
                    .Create(),
                fixture.For<Match>()
                    .With(m => m.Id, "match:2")
                    .With(m => m.MatchResult, fixture.For<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.Live)
                        .Create())
                    .Create(),
            };

            var filteredMatches = liveMatchFilter.FilterClosed(liveMatchFilter.FilterNotStarted(matches)).ToList();

            Assert.Equal(2, filteredMatches.Count);
        }

        [Fact]
        public void FilterAsync_HaveNotStartedMatchesAndValidEventDate_ShouldReturnCorrectList()
        {
            var matches = new List<Match>
            {
                fixture.For<Match>()
                    .With(m => m.Id, "match:1")
                    .With(m => m.MatchResult, fixture.For<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.Closed)
                        .Create())
                    .With(m => m.LatestTimeline,  fixture.For<TimelineEvent>()
                        .With(t => t.Type, EventType.MatchEnded)
                        .With(t => t.Time, DateTimeOffset.Now)
                        .Create())
                    .Create(),
                fixture.For<Match>()
                    .With(m => m.Id, "match:2")
                    .With(m => m.MatchResult, fixture.For<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.Live)
                        .Create())
                    .Create(),
                fixture.For<Match>()
                    .With(m => m.Id, "match:3")
                    .With(m => m.EventDate, DateTimeOffset.Now )
                    .With(m => m.MatchResult, fixture.For<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.NotStarted)
                        .Create())
                    .Create()
            };

            var filteredMatches = liveMatchFilter.FilterClosed(liveMatchFilter.FilterNotStarted(matches)).ToList();

            Assert.Equal(3, filteredMatches.Count);
        }

        [Fact]
        public void FilterAsync_HaveNotStartedMatchesButInvalidEventDate_ShouldReturnCorrectList()
        {
            var matches = new List<Match>
            {
                fixture.For<Match>()
                    .With(m => m.Id, "match:1")
                    .With(m => m.MatchResult, fixture.For<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.Closed)
                        .Create())
                    .With(m => m.LatestTimeline,  fixture.For<TimelineEvent>()
                        .With(t => t.Type, EventType.MatchEnded)
                        .With(t => t.Time, DateTimeOffset.Now)
                        .Create())
                    .Create(),
                fixture.For<Match>()
                    .With(m => m.Id, "match:2")
                    .With(m => m.MatchResult, fixture.For<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.Live)
                        .Create())
                    .Create(),
                fixture.For<Match>()
                    .With(m => m.Id, "match:3")
                    .With(m => m.EventDate, DateTimeOffset.Now )
                    .With(m => m.MatchResult, fixture.For<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.NotStarted)
                        .Create())
                    .Create(),
                fixture.For<Match>()
                    .With(m => m.Id, "match:4")
                    .With(m => m.EventDate, DateTimeOffset.Now - TimeSpan.FromMinutes(11))
                    .With(m => m.MatchResult, fixture.For<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.NotStarted)
                        .Create())
                    .Create(),
                fixture.For<Match>()
                    .With(m => m.Id, "match:5")
                    .With(m => m.EventDate, DateTimeOffset.Now - TimeSpan.FromMinutes(20))
                    .With(m => m.MatchResult, fixture.For<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.NotStarted)
                        .Create())
                    .Create()
            };

            var filteredMatches = liveMatchFilter.FilterClosed(liveMatchFilter.FilterNotStarted(matches)).ToList();

            Assert.Equal(3, filteredMatches.Count);
        }

        [Fact]
        public void FilterAsync_AllValidNotStartedMatches_ShouldReturnCorrectList()
        {
            var matches = new List<Match>
            {
                fixture.For<Match>()
                    .With(m => m.Id, "match:1")
                    .With(m => m.EventDate, DateTimeOffset.Now + TimeSpan.FromMinutes(11))
                    .With(m => m.MatchResult, fixture.For<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.NotStarted)
                        .Create())
                    .Create(),
                fixture.For<Match>()
                    .With(m => m.Id, "match:2")
                    .With(m => m.EventDate, DateTimeOffset.Now + TimeSpan.FromMinutes(9))
                    .With(m => m.MatchResult, fixture.For<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.NotStarted)
                        .Create())
                    .Create(),
                fixture.For<Match>()
                    .With(m => m.Id, "match:3")
                    .With(m => m.EventDate, DateTimeOffset.Now + TimeSpan.FromMinutes(5))
                    .With(m => m.MatchResult, fixture.For<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.NotStarted)
                        .Create())
                    .Create(),
                fixture.For<Match>()
                    .With(m => m.Id, "match:4")
                    .With(m => m.EventDate, DateTimeOffset.Now)
                    .With(m => m.MatchResult, fixture.For<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.NotStarted)
                        .Create())
                    .Create(),
                fixture.For<Match>()
                    .With(m => m.Id, "match:5")
                    .With(m => m.EventDate, DateTimeOffset.Now - TimeSpan.FromMinutes(1))
                    .With(m => m.MatchResult, fixture.For<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.NotStarted)
                        .Create())
                    .Create(),
                fixture.For<Match>()
                    .With(m => m.Id, "match:6")
                    .With(m => m.EventDate, DateTimeOffset.Now - TimeSpan.FromMinutes(2))
                    .With(m => m.MatchResult, fixture.For<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.NotStarted)
                        .Create())
                    .Create(),
                fixture.For<Match>()
                    .With(m => m.Id, "match:7")
                    .With(m => m.EventDate, DateTimeOffset.Now - TimeSpan.FromMinutes(11))
                    .With(m => m.MatchResult, fixture.For<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.NotStarted)
                        .Create())
                    .Create(),
                fixture.For<Match>()
                    .With(m => m.Id, "match:8")
                    .With(m => m.EventDate, DateTimeOffset.Now - TimeSpan.FromMinutes(19))
                    .With(m => m.MatchResult, fixture.For<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.NotStarted)
                        .Create())
                    .Create()
            };

            var filteredMatches = liveMatchFilter.FilterClosed(liveMatchFilter.FilterNotStarted(matches)).ToList();

            Assert.Equal(5, filteredMatches.Count);
        }

        [Fact]
        public void FilterAsync_InvalidClosedMatch_ShouldReturnCorrectList()
        {
            var matches = new List<Match>
            {
                fixture.For<Match>()
                    .With(m => m.Id, "match:1")
                    .With(m => m.MatchResult, fixture.For<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.Closed)
                        .Create())
                    .With(m => m.LatestTimeline,  fixture.For<TimelineEvent>()
                        .With(t => t.Type, EventType.MatchEnded)
                        .With(t => t.Time, DateTimeOffset.Now.AddMinutes(-11) )
                        .Create())
                    .Create(),
                fixture.For<Match>()
                    .With(m => m.Id, "match:2")
                    .With(m => m.EventDate, DateTimeOffset.Now - TimeSpan.FromMinutes(5))
                    .With(m => m.MatchResult, fixture.For<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.NotStarted)
                        .Create())
                    .Create(),
                fixture.For<Match>()
                    .With(m => m.Id, "match:3")
                    .With(m => m.EventDate, DateTimeOffset.Now - TimeSpan.FromMinutes(9))
                    .With(m => m.MatchResult, fixture.For<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.NotStarted)
                        .Create())
                    .Create()
            };

            var filteredMatches = liveMatchFilter.FilterClosed(liveMatchFilter.FilterNotStarted(matches)).ToList();

            Assert.Equal(2, filteredMatches.Count);
        }

        [Fact]
        public void FilterAsync_ClosedMatchButLatestTimelineIsNull_ShouldReturnCorrectList()
        {
            var matches = new List<Match>
            {
                fixture.For<Match>()
                    .With(m => m.Id, "match:3")
                    .With(m => m.MatchResult, fixture.For<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.Closed)
                        .Create())
                    .Create()
            };

            var filteredMatches = liveMatchFilter.FilterClosed(liveMatchFilter.FilterNotStarted(matches)).ToList();

            Assert.Single(filteredMatches);
        }

        [Fact]
        public void FilterAsync_AllClosedMatch_ShouldReturnCorrectList()
        {
            var matches = new List<Match>
            {
                fixture.For<Match>()
                    .With(m => m.Id, "match:1")
                    .With(m => m.MatchResult, fixture.For<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.Closed)
                        .Create())
                    .With(m => m.LatestTimeline,  fixture.For<TimelineEvent>()
                        .With(t => t.Type, EventType.MatchEnded)
                        .With(t => t.Time, DateTimeOffset.Now.AddMinutes(-11) )
                        .Create())
                    .Create(),
                fixture.For<Match>()
                    .With(m => m.Id, "match:2")
                    .With(m => m.MatchResult, fixture.For<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.Closed)
                        .Create())
                    .With(m => m.LatestTimeline,  fixture.For<TimelineEvent>()
                        .With(t => t.Type, EventType.MatchEnded)
                        .With(t => t.Time, DateTimeOffset.Now.AddMinutes(-9) )
                        .Create())
                    .Create(),
                fixture.For<Match>()
                    .With(m => m.Id, "match:3")
                    .With(m => m.MatchResult, fixture.For<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.Closed)
                        .Create())
                    .With(m => m.LatestTimeline,  fixture.For<TimelineEvent>()
                        .With(t => t.Type, EventType.MatchEnded)
                        .With(t => t.Time, DateTimeOffset.Now.AddMinutes(-5) )
                        .Create())
                    .Create(),
                fixture.For<Match>()
                    .With(m => m.Id, "match:4")
                    .With(m => m.MatchResult, fixture.For<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.Closed)
                        .Create())
                    .With(m => m.LatestTimeline,  fixture.For<TimelineEvent>()
                        .With(t => t.Type, EventType.MatchEnded)
                        .With(t => t.Time, DateTimeOffset.Now)
                        .Create())
                    .Create()
            };

            var filteredMatches = liveMatchFilter.FilterClosed(liveMatchFilter.FilterNotStarted(matches)).ToList();

            Assert.Equal(3, filteredMatches.Count);
        }
    }
}
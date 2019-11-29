using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
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
        private readonly ILiveMatchFilter liveMatchFilter;

        public EventDateFilterTests()
        {
            liveMatchFilter = new LiveMatchFilter();
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
                A.Dummy<Match>()
                    .With(m => m.Id, "match:1")
                    .With(m => m.MatchResult,  A.Dummy<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.Closed))
                    .With(m => m.LatestTimeline,   A.Dummy<TimelineEvent>()
                            .With(t => t.Type, EventType.MatchEnded)
                            .With(t => t.Time, DateTimeOffset.Now)),
                A.Dummy<Match>()
                    .With(m => m.Id, "match:2")
                    .With(m => m.MatchResult,  A.Dummy<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.Live)),
            };

            var filteredMatches = liveMatchFilter.FilterClosed(liveMatchFilter.FilterNotStarted(matches)).ToList();

            Assert.Equal(2, filteredMatches.Count);
        }

        [Fact]
        public void FilterAsync_HaveNotStartedMatchesAndValidEventDate_ShouldReturnCorrectList()
        {
            var matches = new List<Match>
            {
                A.Dummy<Match>()
                    .With(m => m.Id, "match:1")
                    .With(m => m.MatchResult,  A.Dummy<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.Closed))
                    .With(m => m.LatestTimeline,   A.Dummy<TimelineEvent>()
                        .With(t => t.Type, EventType.MatchEnded)
                        .With(t => t.Time, DateTimeOffset.Now)),
                A.Dummy<Match>()
                    .With(m => m.Id, "match:2")
                    .With(m => m.MatchResult,  A.Dummy<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.Live)),
                A.Dummy<Match>()
                    .With(m => m.Id, "match:3")
                    .With(m => m.EventDate, DateTimeOffset.Now )
                    .With(m => m.MatchResult,  A.Dummy<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.NotStarted))
            };

            var filteredMatches = liveMatchFilter.FilterClosed(liveMatchFilter.FilterNotStarted(matches)).ToList();

            Assert.Equal(3, filteredMatches.Count);
        }

        [Fact]
        public void FilterAsync_HaveNotStartedMatchesButInvalidEventDate_ShouldReturnCorrectList()
        {
            var matches = new List<Match>
            {
                A.Dummy<Match>()
                    .With(m => m.Id, "match:1")
                    .With(m => m.MatchResult, A.Dummy<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.Closed))
                    .With(m => m.LatestTimeline,  A.Dummy<TimelineEvent>()
                        .With(t => t.Type, EventType.MatchEnded)
                        .With(t => t.Time, DateTimeOffset.Now)),
                A.Dummy<Match>()
                    .With(m => m.Id, "match:2")
                    .With(m => m.MatchResult, A.Dummy<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.Live)),
                A.Dummy<Match>()
                    .With(m => m.Id, "match:3")
                    .With(m => m.EventDate, DateTimeOffset.Now )
                    .With(m => m.MatchResult, A.Dummy<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.NotStarted)),
                A.Dummy<Match>()
                    .With(m => m.Id, "match:4")
                    .With(m => m.EventDate, DateTimeOffset.Now - TimeSpan.FromMinutes(11))
                    .With(m => m.MatchResult, A.Dummy<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.NotStarted)),
                A.Dummy<Match>()
                    .With(m => m.Id, "match:5")
                    .With(m => m.EventDate, DateTimeOffset.Now - TimeSpan.FromMinutes(20))
                    .With(m => m.MatchResult, A.Dummy<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.NotStarted))
            };

            var filteredMatches = liveMatchFilter.FilterClosed(liveMatchFilter.FilterNotStarted(matches)).ToList();

            Assert.Equal(3, filteredMatches.Count);
        }

        [Fact]
        public void FilterAsync_AllValidNotStartedMatches_ShouldReturnCorrectList()
        {
            var matches = new List<Match>
            {
                A.Dummy<Match>()
                    .With(m => m.Id, "match:1")
                    .With(m => m.EventDate, DateTimeOffset.Now + TimeSpan.FromMinutes(11))
                    .With(m => m.MatchResult, A.Dummy<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.NotStarted)),
                A.Dummy<Match>()
                    .With(m => m.Id, "match:2")
                    .With(m => m.EventDate, DateTimeOffset.Now + TimeSpan.FromMinutes(9))
                    .With(m => m.MatchResult, A.Dummy<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.NotStarted)
                        )
                    ,
                A.Dummy<Match>()
                    .With(m => m.Id, "match:3")
                    .With(m => m.EventDate, DateTimeOffset.Now + TimeSpan.FromMinutes(5))
                    .With(m => m.MatchResult, A.Dummy<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.NotStarted)
                        )
                    ,
                A.Dummy<Match>()
                    .With(m => m.Id, "match:4")
                    .With(m => m.EventDate, DateTimeOffset.Now)
                    .With(m => m.MatchResult, A.Dummy<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.NotStarted)
                        )
                    ,
                A.Dummy<Match>()
                    .With(m => m.Id, "match:5")
                    .With(m => m.EventDate, DateTimeOffset.Now - TimeSpan.FromMinutes(1))
                    .With(m => m.MatchResult, A.Dummy<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.NotStarted)
                        )
                    ,
                A.Dummy<Match>()
                    .With(m => m.Id, "match:6")
                    .With(m => m.EventDate, DateTimeOffset.Now - TimeSpan.FromMinutes(2))
                    .With(m => m.MatchResult, A.Dummy<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.NotStarted)
                        )
                    ,
                A.Dummy<Match>()
                    .With(m => m.Id, "match:7")
                    .With(m => m.EventDate, DateTimeOffset.Now - TimeSpan.FromMinutes(11))
                    .With(m => m.MatchResult, A.Dummy<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.NotStarted)
                        )
                    ,
                A.Dummy<Match>()
                    .With(m => m.Id, "match:8")
                    .With(m => m.EventDate, DateTimeOffset.Now - TimeSpan.FromMinutes(19))
                    .With(m => m.MatchResult, A.Dummy<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.NotStarted)
                        )
            };

            var filteredMatches = liveMatchFilter.FilterClosed(liveMatchFilter.FilterNotStarted(matches)).ToList();

            Assert.Equal(5, filteredMatches.Count);
        }

        [Fact]
        public void FilterAsync_ClosedMatchLaterThanTenMinutes_ShouldReturnCorrectList()
        {
            var matches = new List<Match>
            {
                A.Dummy<Match>()
                    .With(m => m.Id, "match:1")
                    .With(m => m.MatchResult, A.Dummy<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.Closed)
                        )
                    .With(m => m.LatestTimeline,  A.Dummy<TimelineEvent>()
                        .With(t => t.Type, EventType.MatchEnded)
                        .With(t => t.Time, DateTimeOffset.Now.AddMinutes(-11) )
                        )
                    ,
                A.Dummy<Match>()
                    .With(m => m.Id, "match:2")
                    .With(m => m.EventDate, DateTimeOffset.Now - TimeSpan.FromMinutes(5))
                    .With(m => m.MatchResult, A.Dummy<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.NotStarted)
                        )
                    ,
                A.Dummy<Match>()
                    .With(m => m.Id, "match:3")
                    .With(m => m.EventDate, DateTimeOffset.Now - TimeSpan.FromMinutes(9))
                    .With(m => m.MatchResult, A.Dummy<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.NotStarted)
                        )
            };

            var filteredMatches = liveMatchFilter.FilterClosed(liveMatchFilter.FilterNotStarted(matches)).ToList();

            Assert.Equal(2, filteredMatches.Count);
        }

        [Fact]
        public void FilterAsync_ClosedMatchButNoTimelines_ShouldRemoveAfter3Hours()
        {
            var matches = new List<Match>
            {
                A.Dummy<Match>()
                    .With(m => m.Id, "match:3")
                    .With(m => m.MatchResult,
                        A.Dummy<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.Closed))
                    .With(m => m.EventDate, DateTimeOffset.UtcNow.AddHours(-4))
                    .With(m => m.LatestTimeline,  null)
            };

            var filteredMatches = liveMatchFilter.FilterClosed(liveMatchFilter.FilterNotStarted(matches)).ToList();

            Assert.Empty(filteredMatches);
        }

        [Fact]
        public void FilterAsync_AllClosedMatch_ShouldReturnCorrectList()
        {
            var matches = new List<Match>
            {
                A.Dummy<Match>()
                    .With(m => m.Id, "match:1")
                    .With(m => m.MatchResult, A.Dummy<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.Closed)
                        )
                    .With(m => m.LatestTimeline,  A.Dummy<TimelineEvent>()
                        .With(t => t.Type, EventType.MatchEnded)
                        .With(t => t.Time, DateTimeOffset.Now.AddMinutes(-11) )
                        )
                    ,
                A.Dummy<Match>()
                    .With(m => m.Id, "match:2")
                    .With(m => m.MatchResult, A.Dummy<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.Closed)
                        )
                    .With(m => m.LatestTimeline,  A.Dummy<TimelineEvent>()
                        .With(t => t.Type, EventType.MatchEnded)
                        .With(t => t.Time, DateTimeOffset.Now.AddMinutes(-9) )
                        )
                    ,
                A.Dummy<Match>()
                    .With(m => m.Id, "match:3")
                    .With(m => m.MatchResult, A.Dummy<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.Closed)
                        )
                    .With(m => m.LatestTimeline,  A.Dummy<TimelineEvent>()
                        .With(t => t.Type, EventType.MatchEnded)
                        .With(t => t.Time, DateTimeOffset.Now.AddMinutes(-5) )
                        )
                    ,
                A.Dummy<Match>()
                    .With(m => m.Id, "match:4")
                    .With(m => m.MatchResult, A.Dummy<MatchResult>()
                        .With(r => r.EventStatus, MatchStatus.Closed)
                        )
                    .With(m => m.LatestTimeline,  A.Dummy<TimelineEvent>()
                        .With(t => t.Type, EventType.MatchEnded)
                        .With(t => t.Time, DateTimeOffset.Now)
                        )
            };

            var filteredMatches = liveMatchFilter.FilterClosed(liveMatchFilter.FilterNotStarted(matches)).ToList();

            Assert.Equal(3, filteredMatches.Count);
        }
    }
}
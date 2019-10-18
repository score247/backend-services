using System;
using System.Collections.Generic;
using System.Linq;
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
            liveMatchFilter = new LiveMatchFilter(new LiveMatchRangeValidator());
        }

        [Fact]
        public void FilterAsync_EmptyList_ShouldReturnEmpty()
        {
            var matches = new List<Match>();

            var filteredMatches = liveMatchFilter.RemoveInvalidNotStarted(matches);

            Assert.Empty(filteredMatches);
        }

        [Fact]
        public void FilterAsync_NotHaveNotStartedMatches_ShouldReturnCorrectList()
        {
            var matches = new List<Match>
            {
                new Match {
                    Id = "match:1",
                    MatchResult = new MatchResult{ EventStatus = MatchStatus.Closed },
                    LatestTimeline = new TimelineEvent{ Type = EventType.MatchEnded, Time = DateTimeOffset.Now } },
                new Match { Id = "match:2", MatchResult = new MatchResult{ EventStatus = MatchStatus.Live }  }
            };

            var filteredMatches = liveMatchFilter.RemoveInvalidClosed(liveMatchFilter.RemoveInvalidNotStarted(matches)).ToList();            

            Assert.Equal(2, filteredMatches.Count);
        }

        [Fact]
        public void FilterAsync_HaveNotStartedMatchesAndValidEventDate_ShouldReturnCorrectList()
        {
            var matches = new List<Match>
            {
                 new Match {
                    Id = "match:1",
                    MatchResult = new MatchResult{ EventStatus = MatchStatus.Closed },
                    LatestTimeline = new TimelineEvent{ Type = EventType.MatchEnded, Time = DateTimeOffset.Now } },
                new Match { Id = "match:2", MatchResult = new MatchResult{ EventStatus = MatchStatus.Live }  },
                new Match { Id = "match:2", MatchResult = new MatchResult{ EventStatus = MatchStatus.NotStarted }, EventDate = DateTimeOffset.Now  },

            };

            var filteredMatches = liveMatchFilter.RemoveInvalidClosed(liveMatchFilter.RemoveInvalidNotStarted(matches)).ToList();

            Assert.Equal(3, filteredMatches.Count);
        }

        [Fact]
        public void FilterAsync_HaveNotStartedMatchesButInvalidEventDate_ShouldReturnCorrectList()
        {
            var matches = new List<Match>
            {
                 new Match {
                    Id = "match:1",
                    MatchResult = new MatchResult{ EventStatus = MatchStatus.Closed },
                    LatestTimeline = new TimelineEvent{ Type = EventType.MatchEnded, Time = DateTimeOffset.Now } },
                new Match { Id = "match:2", MatchResult = new MatchResult{ EventStatus = MatchStatus.Live }  },
                new Match { Id = "match:2", MatchResult = new MatchResult{ EventStatus = MatchStatus.NotStarted }, EventDate = DateTimeOffset.Now  },
                new Match { Id = "match:2", MatchResult = new MatchResult{ EventStatus = MatchStatus.NotStarted }, EventDate = (DateTimeOffset.Now - TimeSpan.FromMinutes(11))  },
                new Match { Id = "match:2", MatchResult = new MatchResult{ EventStatus = MatchStatus.NotStarted }, EventDate = (DateTimeOffset.Now - TimeSpan.FromMinutes(20))  },
            };

            var filteredMatches = liveMatchFilter.RemoveInvalidClosed(liveMatchFilter.RemoveInvalidNotStarted(matches)).ToList();

            Assert.Equal(3, filteredMatches.Count);
        }

        [Fact]
        public void FilterAsync_AllValidNotStartedMatches_ShouldReturnCorrectList()
        {
            var matches = new List<Match>
            {
                new Match { Id = "match:1", MatchResult = new MatchResult{ EventStatus = MatchStatus.NotStarted }, EventDate = DateTimeOffset.Now  + TimeSpan.FromMinutes(11) },
                new Match { Id = "match:2", MatchResult = new MatchResult{ EventStatus = MatchStatus.NotStarted }, EventDate = DateTimeOffset.Now  + TimeSpan.FromMinutes(9) },
                new Match { Id = "match:3", MatchResult = new MatchResult{ EventStatus = MatchStatus.NotStarted }, EventDate = DateTimeOffset.Now  + TimeSpan.FromMinutes(5) },
                new Match { Id = "match:4", MatchResult = new MatchResult{ EventStatus = MatchStatus.NotStarted }, EventDate = DateTimeOffset.Now  },
                new Match { Id = "match:5", MatchResult = new MatchResult{ EventStatus = MatchStatus.NotStarted }, EventDate = (DateTimeOffset.Now - TimeSpan.FromMinutes(1))  },
                new Match { Id = "match:6", MatchResult = new MatchResult{ EventStatus = MatchStatus.NotStarted }, EventDate = (DateTimeOffset.Now - TimeSpan.FromMinutes(2))  },
                new Match { Id = "match:7", MatchResult = new MatchResult{ EventStatus = MatchStatus.NotStarted }, EventDate = (DateTimeOffset.Now - TimeSpan.FromMinutes(11))  },
                new Match { Id = "match:8", MatchResult = new MatchResult{ EventStatus = MatchStatus.NotStarted }, EventDate = (DateTimeOffset.Now - TimeSpan.FromMinutes(19))  },
            };

            var filteredMatches = liveMatchFilter.RemoveInvalidClosed(liveMatchFilter.RemoveInvalidNotStarted(matches)).ToList();

            Assert.Equal(5, filteredMatches.Count);
        }

        [Fact]
        public void FilterAsync_InvalidClosedMatch_ShouldReturnCorrectList()
        {
            var matches = new List<Match>
            {
                new Match {
                    Id = "match:1",
                    MatchResult = new MatchResult{ EventStatus = MatchStatus.Closed },
                    LatestTimeline = new TimelineEvent{ Type = EventType.MatchEnded, Time = DateTimeOffset.Now.AddMinutes(-11) } },
                new Match {
                    Id = "match:2", MatchResult = new MatchResult{ EventStatus = MatchStatus.NotStarted },
                    EventDate = (DateTimeOffset.Now - TimeSpan.FromMinutes(5))  },
                new Match {
                    Id = "match:3", MatchResult = new MatchResult{ EventStatus = MatchStatus.NotStarted },
                    EventDate = (DateTimeOffset.Now - TimeSpan.FromMinutes(9))  },
            };

            var filteredMatches = liveMatchFilter.RemoveInvalidClosed(liveMatchFilter.RemoveInvalidNotStarted(matches)).ToList();

            Assert.Equal(2, filteredMatches.Count);
        }

        [Fact]
        public void FilterAsync_ClosedMatchButLatestTimelineIsNull_ShouldReturnCorrectList()
        {
            var matches = new List<Match>
            {
                new Match {
                    Id = "match:1",
                    MatchResult = new MatchResult{ EventStatus = MatchStatus.Closed } }                
            };

            var filteredMatches = liveMatchFilter.RemoveInvalidClosed(liveMatchFilter.RemoveInvalidNotStarted(matches)).ToList();

            Assert.Single(filteredMatches);
        }

        [Fact]
        public void FilterAsync_AllClosedMatch_ShouldReturnCorrectList()
        {
            var matches = new List<Match>
            {
                new Match {
                    Id = "match:1", MatchResult = new MatchResult{ EventStatus = MatchStatus.Closed },
                    LatestTimeline = new TimelineEvent{ Type = EventType.MatchEnded, Time = DateTimeOffset.Now - TimeSpan.FromMinutes(11) } },
                new Match {
                    Id = "match:2", MatchResult = new MatchResult{ EventStatus = MatchStatus.Closed },
                    LatestTimeline = new TimelineEvent{ Type = EventType.MatchEnded, Time = DateTimeOffset.Now - TimeSpan.FromMinutes(9) } },
                 new Match {
                    Id = "match:3", MatchResult = new MatchResult{ EventStatus = MatchStatus.Closed },
                    LatestTimeline = new TimelineEvent{ Type = EventType.MatchEnded, Time = DateTimeOffset.Now - TimeSpan.FromMinutes(5) } },
                  new Match {
                    Id = "match:4", MatchResult = new MatchResult{ EventStatus = MatchStatus.Closed },
                    LatestTimeline = new TimelineEvent{ Type = EventType.MatchEnded, Time = DateTimeOffset.Now } },
            };

            var filteredMatches = liveMatchFilter.RemoveInvalidClosed(liveMatchFilter.RemoveInvalidNotStarted(matches)).ToList();

            Assert.Equal(3, filteredMatches.Count);
        }
    }
}

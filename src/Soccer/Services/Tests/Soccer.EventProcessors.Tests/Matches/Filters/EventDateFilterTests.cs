﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.EventProcessors._Shared.Filters;
using Soccer.EventProcessors.Matches.Filters;
using Xunit;

namespace Soccer.EventProcessors.Tests.Matches.Filters
{
    [Trait("Soccer.EventProcessors", "EventDateFilter")]
    public class EventDateFilterTests
    {

        private readonly IFilter<IEnumerable<Match>, IEnumerable<Match>> eventDateFilter;

        public EventDateFilterTests()
        {
            eventDateFilter = new MatchEventDateFilter();
        }

        [Fact]
        public async Task FilterAsync_EmptyList_ShouldReturnEmpty()
        {
            var matches = new List<Match>();

            var filteredMatches = await eventDateFilter.FilterAsync(matches);

            Assert.Empty(filteredMatches);
        }

        [Fact]
        public async Task FilterAsync_NotHaveNotStartedMatches_ShouldReturnCorrectList()
        {
            var matches = new List<Match>
            {
                new Match { Id = "match:1", MatchResult = new MatchResult{ EventStatus = MatchStatus.Closed } },
                new Match { Id = "match:2", MatchResult = new MatchResult{ EventStatus = MatchStatus.Live }  }
            };

            var filteredMatches = (await eventDateFilter.FilterAsync(matches)).ToList();

            Assert.Equal(2, filteredMatches.Count);
        }

        [Fact]
        public async Task FilterAsync_HaveNotStartedMatchesAndValidEventDate_ShouldReturnCorrectList()
        {
            var matches = new List<Match>
            {
                new Match { Id = "match:1", MatchResult = new MatchResult{ EventStatus = MatchStatus.Closed } },
                new Match { Id = "match:2", MatchResult = new MatchResult{ EventStatus = MatchStatus.Live }  },
                new Match { Id = "match:2", MatchResult = new MatchResult{ EventStatus = MatchStatus.NotStarted }, EventDate = DateTimeOffset.Now  },

            };

            var filteredMatches = (await eventDateFilter.FilterAsync(matches)).ToList();

            Assert.Equal(3, filteredMatches.Count);
        }

        [Fact]
        public async Task FilterAsync_HaveNotStartedMatchesButInvalidEventDate_ShouldReturnCorrectList()
        {
            var matches = new List<Match>
            {
                new Match { Id = "match:1", MatchResult = new MatchResult{ EventStatus = MatchStatus.Closed } },
                new Match { Id = "match:2", MatchResult = new MatchResult{ EventStatus = MatchStatus.Live }  },
                new Match { Id = "match:2", MatchResult = new MatchResult{ EventStatus = MatchStatus.NotStarted }, EventDate = DateTimeOffset.Now  },
                new Match { Id = "match:2", MatchResult = new MatchResult{ EventStatus = MatchStatus.NotStarted }, EventDate = (DateTimeOffset.Now - TimeSpan.FromMinutes(11))  },
                new Match { Id = "match:2", MatchResult = new MatchResult{ EventStatus = MatchStatus.NotStarted }, EventDate = (DateTimeOffset.Now - TimeSpan.FromMinutes(20))  },
            };

            var filteredMatches = (await eventDateFilter.FilterAsync(matches)).ToList();

            Assert.Equal(3, filteredMatches.Count);
        }

        [Fact]
        public async Task FilterAsync_AllValidNotStartedMatches_ShouldReturnFullList()
        {
            var matches = new List<Match>
            {
                new Match { Id = "match:2", MatchResult = new MatchResult{ EventStatus = MatchStatus.NotStarted }, EventDate = DateTimeOffset.Now  },
                new Match { Id = "match:2", MatchResult = new MatchResult{ EventStatus = MatchStatus.NotStarted }, EventDate = (DateTimeOffset.Now - TimeSpan.FromMinutes(5))  },
                new Match { Id = "match:2", MatchResult = new MatchResult{ EventStatus = MatchStatus.NotStarted }, EventDate = (DateTimeOffset.Now - TimeSpan.FromMinutes(9))  },
            };

            var filteredMatches = (await eventDateFilter.FilterAsync(matches)).ToList();

            Assert.Equal(3, filteredMatches.Count);
        }
    }
}

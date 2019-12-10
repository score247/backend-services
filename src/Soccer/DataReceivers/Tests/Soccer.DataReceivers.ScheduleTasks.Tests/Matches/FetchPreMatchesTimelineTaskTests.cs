using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using MassTransit;
using NSubstitute;
using Score247.Shared.Tests;
using Soccer.Core.Matches.Models;
using Soccer.Core.Matches.QueueMessages;
using Soccer.Core.Shared.Enumerations;
using Soccer.Core.Timelines.Models;
using Soccer.DataProviders.Matches.Services;
using Soccer.DataReceivers.ScheduleTasks.Matches;
using Xunit;

namespace Soccer.DataReceivers.ScheduleTasks.Tests.Matches
{
    public class FetchPreMatchesTimelineTaskTests
    {
        private readonly ITimelineService timelineService;
        private readonly IBus messageBus;

        private readonly FetchPreMatchesTimelineTask fetchPreMatchesTimelineTask;

        public FetchPreMatchesTimelineTaskTests()
        {
            timelineService = Substitute.For<ITimelineService>();
            messageBus = Substitute.For<IBus>();

            fetchPreMatchesTimelineTask = new FetchPreMatchesTimelineTask(messageBus, timelineService);
        }

        [Fact]
        public async Task FetchPreMatchTimeline_NoData_NotPublishAnyMessages()
        {
            await fetchPreMatchesTimelineTask.FetchPreMatchTimeline(new List<Match>());

            await messageBus.DidNotReceive().Publish<IMatchUpdatedCoverageInfo>(Arg.Any<MatchUpdatedCoverageInfo>());
        }

        [Fact]
        public async Task FetchPreMatchTimeline_FetchTimelineApi()
        {
            var matches = A.CollectionOfDummy<Match>(10);

            await fetchPreMatchesTimelineTask.FetchPreMatchTimeline(matches);

            await timelineService.Received(10).GetTimelines(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Language>());
            await messageBus.DidNotReceive().Publish<IMatchUpdatedCoverageInfo>(Arg.Any<MatchUpdatedCoverageInfo>());
        }

        [Fact]
        public async Task FetchPreMatchTimeline_PublishMatchUpdatedCoverageInfo()
        {
            var match = A.Dummy<Match>()
                .With(match => match.Id, "match:id")
                .With(match => match.Id, "match:id");

            timelineService
                .GetTimelines(match.Id, Arg.Any<string>(), Arg.Any<Language>())
                .Returns(new Tuple<Match, IEnumerable<TimelineCommentary>>(
                    A.Dummy<Match>().With(match => match.Id, "match:id"), 
                    Enumerable.Empty<TimelineCommentary>())); 

            await fetchPreMatchesTimelineTask.FetchPreMatchTimeline(new List<Match> { match });
            
            await messageBus.Received(1).Publish<IMatchUpdatedCoverageInfo>(Arg.Any<MatchUpdatedCoverageInfo>());
        }
    }
}

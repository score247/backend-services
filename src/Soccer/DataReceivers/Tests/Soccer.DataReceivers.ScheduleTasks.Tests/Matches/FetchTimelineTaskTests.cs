using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using MassTransit;
using NSubstitute;
using Score247.Shared.Tests;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Matches.Models;
using Soccer.Core.Matches.QueueMessages;
using Soccer.Core.Shared.Enumerations;
using Soccer.Core.Teams.Models;
using Soccer.Core.Teams.QueueMessages;
using Soccer.Core.Timeline.Models;
using Soccer.Core.Timeline.QueueMessages;
using Soccer.DataProviders.Matches.Services;
using Soccer.DataReceivers.ScheduleTasks.Matches;
using Xunit;

namespace Soccer.DataReceivers.ScheduleTasks.Tests.Matches
{
    [Trait("Soccer.DataReceivers", "FetchTimelineTask")]
    public class FetchTimelineTaskTests
    {
        private readonly ITimelineService timelineService;
        private readonly IBus messageBus;
        private readonly FetchTimelineTask fetchTimelineTask;
        private static readonly Fixture fixture = new Fixture();

        public FetchTimelineTaskTests()
        {
            timelineService = Substitute.For<ITimelineService>();
            messageBus = Substitute.For<IBus>();

            fetchTimelineTask = new FetchTimelineTask(messageBus, timelineService);
        }

        [Fact]
        public async Task FetchTimelines_TeamsIsNull_ShouldNotPublishMatchUpdatedConditionsMessage()
        {
            // Arrange

            // Act
            await fetchTimelineTask.FetchTimelines("sr:match", "eu", Language.en_US);

            // Assert
            await messageBus.DidNotReceive().Publish<IMatchUpdatedConditionsMessage>(Arg.Any<MatchUpdatedConditionsMessage>());
        }

        [Fact]
        public async Task FetchTimelines_HasReferee_ShouldPublishMatchUpdatedConditionsMessage()
        {
            // Arrange
            var match = fixture.For<Match>()
                .With(m => m.Teams, new List<Team>())
                .With(m => m.Referee, "AAA")
                .Create();
            timelineService.GetTimelines("sr:match", "eu", Language.en_US).Returns(match);

            // Act
            await fetchTimelineTask.FetchTimelines("sr:match", "eu", Language.en_US);

            // Assert
            await messageBus.Received(1).Publish<IMatchUpdatedConditionsMessage>(Arg.Is<MatchUpdatedConditionsMessage>(m => m.Referee == "AAA"));
        }

        [Fact]
        public async Task FetchTimelines_HasAttendance_ShouldPublishMatchUpdatedConditionsMessage()
        {
            // Arrange
            var match = fixture.For<Match>()
                .With(m => m.Teams, new List<Team>())
                .With(m => m.Attendance, 10000)
                .Create();
            timelineService.GetTimelines("sr:match", "eu", Language.en_US).Returns(match);

            // Act
            await fetchTimelineTask.FetchTimelines("sr:match", "eu", Language.en_US);

            // Assert
            await messageBus.Received(1).Publish<IMatchUpdatedConditionsMessage>(Arg.Is<MatchUpdatedConditionsMessage>(m => m.Attendance == 10000));
        }

        [Fact]
        public async Task FetchTimelines_HasBothAttendanceAndReferee_ShouldPublishMatchUpdatedConditionsMessage()
        {
            // Arrange
            var match = fixture.For<Match>()
                .With(m => m.Teams, new List<Team>())
                .With(m => m.Attendance, 10000)
                .With(m => m.Referee, "AAA")
                .Create();

            timelineService.GetTimelines("sr:match", "eu", Language.en_US).Returns(match);

            // Act
            await fetchTimelineTask.FetchTimelines("sr:match", "eu", Language.en_US);

            // Assert
            await messageBus.Received(1).Publish<IMatchUpdatedConditionsMessage>(Arg.Is<MatchUpdatedConditionsMessage>(
                m => m.Attendance == 10000 && m.Referee == "AAA"));
        }

        [Fact]
        public async Task FetchTimelines_NotHaveRef_ShouldNotPublishMatchUpdatedConditionsMessage()
        {
            // Arrange
            var match = fixture.For<Match>()
                .With(m => m.Teams, new List<Team>())
                .With(m => m.Referee, null)
                .With(m => m.Attendance, 0)
                .Create();
            timelineService.GetTimelines("sr:match", "eu", Language.en_US).Returns(match);

            // Act
            await fetchTimelineTask.FetchTimelines("sr:match", "eu", Language.en_US);

            // Assert
            await messageBus.DidNotReceive().Publish<IMatchUpdatedConditionsMessage>(Arg.Any<MatchUpdatedConditionsMessage>());
        }

        [Fact]
        public async Task FetchTimelines_TeamStatisticNull_ShouldNotPublishTeamStatisticMessage()
        {
            // Arrange
            var match = fixture.For<Match>()
                .With(m => m.Teams, new List<Team>
                {
                    new Team("", "AC Milan", true),
                    new Team("", "Juventus")
                })
                .Create();
            timelineService.GetTimelines("sr:match", "eu", Language.en_US).Returns(match);

            // Act
            await fetchTimelineTask.FetchTimelines("sr:match", "eu", Language.en_US);

            // Assert
            await messageBus.DidNotReceive().Publish<ITeamStatisticUpdatedMessage>(Arg.Any<TeamStatisticUpdatedMessage>());
        }

        [Fact]
        public async Task FetchTimelines_BothTeamStatisticNotNull_ShouldPublishTeamStatisticMessage()
        {
            // Arrange
            var match = fixture.For<Match>()
                .With(m => m.Teams, new List<Team>
                {
                    new Team("", "AC Milan", true, new TeamStatistic(0, 2)),
                    new Team("", "Juventus", false, new TeamStatistic(0, 0))
                })
                .Create();
            timelineService.GetTimelines("sr:match", "eu", Language.en_US).Returns(match);

            // Act
            await fetchTimelineTask.FetchTimelines("sr:match", "eu", Language.en_US);

            // Assert
            await messageBus.Received(2).Publish<ITeamStatisticUpdatedMessage>(Arg.Any<TeamStatisticUpdatedMessage>());
        }

        [Fact]
        public async Task FetchTimelines_TeamStatisticNotNull_ShouldPublishTeamStatisticMessage()
        {
            // Arrange
            var match = fixture.For<Match>()
                .With(m => m.Teams, new List<Team>
                {
                    new Team("", "AC Milan", true, new TeamStatistic(0, 2)),
                    new Team("", "Juventus", false, null)
                })
                .Create();
            timelineService.GetTimelines("sr:match", "eu", Language.en_US).Returns(match);

            // Act
            await fetchTimelineTask.FetchTimelines("sr:match", "eu", Language.en_US);

            // Assert
            await messageBus.Received().Publish<ITeamStatisticUpdatedMessage>(Arg.Any<TeamStatisticUpdatedMessage>());
        }

        [Fact]
        public async Task FetchTimelines_TimelinesNull_ShouldNotPublishMatchTimelinesFetchedMessage()
        {
            // Arrange
            var match = fixture.For<Match>()
                .With(m => m.Teams, new List<Team>
                {
                    new Team("", "AC Milan", true, new TeamStatistic(0, 2)),
                    new Team("", "Juventus", false, new TeamStatistic(0, 0))
                })
                .Create();

            match.TimeLines = null;
            timelineService.GetTimelines("sr:match", "eu", Language.en_US).Returns(match);

            // Act
            await fetchTimelineTask.FetchTimelines("sr:match", "eu", Language.en_US);

            // Assert
            await messageBus.DidNotReceive().Publish<IMatchTimelinesFetchedMessage>(Arg.Any<MatchTimelinesFetchedMessage>());
        }

        [Fact]
        public async Task FetchTimelines_TimelinesEmpty_ShouldNotPublishMatchTimelinesFetchedMessage()
        {
            // Arrange
            var match = fixture.For<Match>()
                .With(m => m.Teams, new List<Team>
                {
                    new Team("", "AC Milan", true, new TeamStatistic(0, 2)),
                    new Team("", "Juventus", false, new TeamStatistic(0, 0))
                })
                .With(m => m.TimeLines, new List<TimelineEvent>())
                .Create();
            match.TimeLines = Enumerable.Empty<TimelineEvent>();
            timelineService.GetTimelines("sr:match", "eu", Language.en_US).Returns(match);

            // Act
            await fetchTimelineTask.FetchTimelines("sr:match", "eu", Language.en_US);

            // Assert
            await messageBus.DidNotReceive().Publish<IMatchTimelinesFetchedMessage>(Arg.Any<MatchTimelinesFetchedMessage>());
        }

        [Fact]
        public async Task FetchTimelines_TimelinesNotEmpty_ShouldPublishMatchTimelinesFetchedMessage()
        {
            // Arrange
            var match = fixture.For<Match>()
                .With(m => m.Teams, new List<Team>
                {
                    new Team("", "AC Milan", true, new TeamStatistic(0, 2)),
                    new Team("", "Juventus", false, new TeamStatistic(0, 0))
                })
                .With(m => m.TimeLines, new List<TimelineEvent>
                {
                    fixture.For<TimelineEvent>().With(t => t.Type, EventType.MatchStarted).Create()
                })
                .Create();
            timelineService.GetTimelines("sr:match", "eu", Language.en_US).Returns(match);

            // Act
            await fetchTimelineTask.FetchTimelines("sr:match", "eu", Language.en_US);

            // Assert
            await messageBus.Received(1).Publish<IMatchTimelinesFetchedMessage>(Arg.Any<MatchTimelinesFetchedMessage>());
        }

        [Fact]
        public async Task FetchTimelines_CommentariesNull_ShouldNotPublishMatchCommentaryFetchedMessage()
        {
            // Arrange
            var match = fixture.For<Match>()
                .With(m => m.Teams, new List<Team>
                {
                    new Team("", "AC Milan", true, new TeamStatistic(0, 2)),
                    new Team("", "Juventus", false, new TeamStatistic(0, 0))
                })
                .With(m => m.TimelineCommentaries, null)
                .Create();
            timelineService.GetTimelines("sr:match", "eu", Language.en_US).Returns(match);

            // Act
            await fetchTimelineTask.FetchTimelines("sr:match", "eu", Language.en_US);

            // Assert
            await messageBus.DidNotReceive().Publish<IMatchCommentaryFetchedMessage>(Arg.Any<MatchCommentaryFetchedMessage>());
        }

        [Fact]
        public async Task FetchTimelines_CommentariesEmpty_ShouldNotPublishMatchCommentaryFetchedMessage()
        {
            // Arrange
            var match = fixture.For<Match>()
                .With(m => m.Teams, new List<Team>
                {
                    new Team("", "AC Milan", true, new TeamStatistic(0, 2)),
                    new Team("", "Juventus", false, new TeamStatistic(0, 0))
                })
                .With(m => m.TimelineCommentaries, Enumerable.Empty<TimelineCommentary>())
                .With(m => m.TimeLines, new List<TimelineEvent>
                {
                    fixture.For<TimelineEvent>()
                        .With(t => t.Type, EventType.MatchStarted)
                        .With(t => t.Commentaries, new List<Commentary>())
                        .Create()
                })
                .Create();
            timelineService.GetTimelines("sr:match", "eu", Language.en_US).Returns(match);

            // Act
            await fetchTimelineTask.FetchTimelines("sr:match", "eu", Language.en_US);

            // Assert
            await messageBus.DidNotReceive().Publish<IMatchCommentaryFetchedMessage>(Arg.Any<MatchCommentaryFetchedMessage>());
        }

        [Fact]
        public async Task FetchTimelines_CommentariesNotEmpty_ShouldPublishMatchCommentaryFetchedMessage()
        {
            // Arrange
            var match = fixture.For<Match>()
                .With(m => m.League, new League("sr:league", ""))
                .With(m => m.Teams, new List<Team>
                {
                    new Team("", "AC Milan", true, new TeamStatistic(0, 2)),
                    new Team("", "Juventus", false, new TeamStatistic(0, 0))
                })
                .With(m => m.TimeLines, new List<TimelineEvent>
                {
                    fixture.For<TimelineEvent>()
                        .With(t => t.Id, "1")
                        .With(t => t.Type, EventType.MatchStarted)
                        .Create(),
                    fixture.For<TimelineEvent>()
                        .With(t => t.Id, "2")
                        .With(t => t.Type, EventType.ScoreChange)
                        .Create()
                })
                .With(m => m.TimelineCommentaries, new List<TimelineCommentary> {
                    new TimelineCommentary (1, new List<Commentary>
                    {
                        new Commentary("match has started")
                    }),
                    new TimelineCommentary (1, new List<Commentary>
                    {
                        new Commentary( "Goal! Cucuta Deportivo FC have got their heads in front thanks to a James Castro strike."),
                        new Commentary("Carmelo Valencia with an assist there.")
                    })})
                .Create();
            timelineService.GetTimelines("sr:match", "eu", Language.en_US).Returns(match);

            // Act
            await fetchTimelineTask.FetchTimelines("sr:match", "eu", Language.en_US);

            // Assert
            await messageBus.Received(2).Publish<IMatchCommentaryFetchedMessage>(Arg.Any<MatchCommentaryFetchedMessage>());
        }
    }
}
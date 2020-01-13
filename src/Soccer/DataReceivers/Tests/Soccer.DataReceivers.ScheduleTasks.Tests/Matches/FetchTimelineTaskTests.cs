using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Hangfire;
using MassTransit;
using NSubstitute;
using Score247.Shared.Tests;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Matches.Models;
using Soccer.Core.Matches.QueueMessages;
using Soccer.Core.Shared.Enumerations;
using Soccer.Core.Teams.Models;
using Soccer.Core.Teams.QueueMessages;
using Soccer.Core.Timelines.Models;
using Soccer.Core.Timelines.QueueMessages;
using Soccer.DataProviders.Matches.Services;
using Soccer.DataReceivers.ScheduleTasks.Matches;
using Xunit;

namespace Soccer.DataReceivers.ScheduleTasks.Tests.Matches
{
    [Trait("Soccer.DataReceivers", "FetchTimelineTask")]
    public class FetchTimelineTaskTests
    {
        private static readonly League WorldCupLeague = new League("1", "World Cup");
        private readonly ITimelineService timelineService;
        private readonly IBus messageBus;
        private readonly FetchTimelineTask fetchTimelineTask;
        private readonly IBackgroundJobClient jobClient;

        public FetchTimelineTaskTests()
        {
            timelineService = Substitute.For<ITimelineService>();
            messageBus = Substitute.For<IBus>();
            jobClient = Substitute.For<IBackgroundJobClient>();

            fetchTimelineTask = new FetchTimelineTask(messageBus, timelineService, jobClient);
        }

        [Fact]
        public async Task FetchTimelines_TeamsIsNull_ShouldNotPublishMatchUpdatedConditionsMessage()
        {
            // Arrange

            // Act
            await fetchTimelineTask.FetchTimelineEvents("sr:match", "eu", Language.en_US);

            // Assert
            await messageBus.DidNotReceive().Publish<IMatchUpdatedConditionsMessage>(Arg.Any<MatchUpdatedConditionsMessage>());
        }

        [Fact]
        public async Task FetchTimelines_HasReferee_ShouldPublishMatchUpdatedConditionsMessage()
        {
            // Arrange
            var match = A.Dummy<Match>()
                .With(m => m.Teams, new List<Team>())
                .With(m => m.Referee, "AAA")
                .With(m => m.League, WorldCupLeague);

            timelineService.GetTimelineEvents("sr:match", "eu", Language.en_US)
                .Returns(new Tuple<Match, IEnumerable<TimelineCommentary>>(match, Enumerable.Empty<TimelineCommentary>()));

            // Act
            await fetchTimelineTask.FetchTimelineEvents("sr:match", "eu", Language.en_US);

            // Assert
            await messageBus.Received(1).Publish<IMatchUpdatedConditionsMessage>(Arg.Is<MatchUpdatedConditionsMessage>(m => m.Referee == "AAA"));
        }

        [Fact]
        public async Task FetchTimelines_HasAttendance_ShouldPublishMatchUpdatedConditionsMessage()
        {
            // Arrange
            var match = A.Dummy<Match>()
                .With(m => m.Attendance, 10000)
                .With(m => m.League, WorldCupLeague);
            timelineService.GetTimelineEvents("sr:match", "eu", Language.en_US)
                .Returns(new Tuple<Match, IEnumerable<TimelineCommentary>>(match, Enumerable.Empty<TimelineCommentary>()));

            // Act
            await fetchTimelineTask.FetchTimelineEvents("sr:match", "eu", Language.en_US);

            // Assert
            await messageBus.Received(1).Publish<IMatchUpdatedConditionsMessage>(Arg.Is<MatchUpdatedConditionsMessage>(m => m.Attendance == 10000));
        }

        [Fact]
        public async Task FetchTimelines_HasBothAttendanceAndReferee_ShouldPublishMatchUpdatedConditionsMessage()
        {
            // Arrange
            var match = A.Dummy<Match>()
                .With(m => m.Teams, new List<Team>())
                .With(m => m.Attendance, 10000)
                .With(m => m.Referee, "AAA")
                .With(m => m.League, WorldCupLeague);

            timelineService.GetTimelineEvents("sr:match", "eu", Language.en_US)
                .Returns(new Tuple<Match, IEnumerable<TimelineCommentary>>(match, Enumerable.Empty<TimelineCommentary>()));

            // Act
            await fetchTimelineTask.FetchTimelineEvents("sr:match", "eu", Language.en_US);

            // Assert
            await messageBus.Received(1).Publish<IMatchUpdatedConditionsMessage>(Arg.Is<MatchUpdatedConditionsMessage>(
                m => m.Attendance == 10000 && m.Referee == "AAA"));
        }

        [Fact]
        public async Task FetchTimelines_NotHaveRef_ShouldNotPublishMatchUpdatedConditionsMessage()
        {
            // Arrange
            var match = A.Dummy<Match>()
                .With(m => m.Teams, new List<Team>())
                .With(m => m.Referee, null)
                .With(m => m.Attendance, 0)
                .With(m => m.League, WorldCupLeague);
            timelineService.GetTimelineEvents("sr:match", "eu", Language.en_US)
                .Returns(new Tuple<Match, IEnumerable<TimelineCommentary>>(match, Enumerable.Empty<TimelineCommentary>()));

            // Act
            await fetchTimelineTask.FetchTimelineEvents("sr:match", "eu", Language.en_US);

            // Assert
            await messageBus.DidNotReceive().Publish<IMatchUpdatedConditionsMessage>(Arg.Any<MatchUpdatedConditionsMessage>());
        }

        [Fact]
        public async Task FetchTimelines_TeamStatisticNull_ShouldNotPublishTeamStatisticMessage()
        {
            // Arrange
            var match = A.Dummy<Match>()
                .With(m => m.League, WorldCupLeague)
                .With(m => m.Teams, new List<Team>
                {
                    A.Dummy<Team>()
                        .With(t => t.Name, "AC Milan")
                        .With(t => t.IsHome, true)
                        .With(t => t.Statistic, null),
                    A.Dummy<Team>()
                        .With(t => t.Name, "Juventus")
                        .With(t => t.IsHome, false)
                        .With(t => t.Statistic, null),
                });
            timelineService.GetTimelineEvents("sr:match", "eu", Language.en_US)
                .Returns(new Tuple<Match, IEnumerable<TimelineCommentary>>(match, Enumerable.Empty<TimelineCommentary>()));

            // Act
            await fetchTimelineTask.FetchTimelineEvents("sr:match", "eu", Language.en_US);

            // Assert
            await messageBus.DidNotReceive().Publish<ITeamStatisticUpdatedMessage>(Arg.Any<TeamStatisticUpdatedMessage>());
        }

        ////[Fact]
        ////public async Task FetchTimelines_BothTeamStatisticNotNull_ShouldPublishTeamStatisticMessage()
        ////{
        ////    // Arrange
        ////    var match = A.Dummy<Match>()
        ////        .With(m => m.League, WorldCupLeague)
        ////        .With(m => m.Teams, new List<Team>
        ////        {
        ////            A.Dummy<Team>()
        ////                .With(t => t.Name, "AC Milan")
        ////                .With(t => t.IsHome, true)
        ////                .With(t => t.Statistic, new TeamStatistic(0, 2)),
        ////            A.Dummy<Team>()
        ////                .With(t => t.Name, "Juventus")
        ////                .With(t => t.IsHome, false)
        ////                .With(t => t.Statistic, new TeamStatistic(0, 0)),
        ////        });

        ////    timelineService.GetTimelines("sr:match", "eu", Language.en_US)
        ////        .Returns(new Tuple<Match, IEnumerable<TimelineCommentary>>(match, Enumerable.Empty<TimelineCommentary>()));

        ////    // Act
        ////    await fetchTimelineTask.FetchTimelines("sr:match", "eu", Language.en_US);

        ////    // Assert
        ////    jobClient.Received(2).Schedule(Arg.Any<Expression<Action<Func<IFetchTimelineTask, Task>>>>(), Arg.Any<TimeSpan>());
        ////}

        ////[Fact]
        ////public async Task FetchTimelines_TeamStatisticNotNull_ShouldPublishTeamStatisticMessage()
        ////{
        ////    // Arrange
        ////    var match = A.Dummy<Match>()
        ////        .With(m => m.League, WorldCupLeague)
        ////        .With(m => m.Teams, new List<Team>
        ////        {
        ////            A.Dummy<Team>()
        ////                .With(t => t.Name, "AC Milan")
        ////                .With(t => t.IsHome, true)
        ////                .With(t => t.Statistic, new TeamStatistic(0, 2)),
        ////            A.Dummy<Team>()
        ////                .With(t => t.Name, "Juventus")
        ////                .With(t => t.IsHome, false)
        ////                .With(t => t.Statistic, null),
        ////        });
        ////    timelineService.GetTimelines("sr:match", "eu", Language.en_US)
        ////        .Returns(new Tuple<Match, IEnumerable<TimelineCommentary>>(match, Enumerable.Empty<TimelineCommentary>()));

        ////    // Act
        ////    await fetchTimelineTask.FetchTimelines("sr:match", "eu", Language.en_US);

        ////    // Assert
        ////    await messageBus.Received().Publish<ITeamStatisticUpdatedMessage>(Arg.Any<TeamStatisticUpdatedMessage>());
        ////}

        [Fact]
        public async Task FetchTimelines_TimelinesNull_ShouldNotPublishMatchTimelinesFetchedMessage()
        {
            // Arrange
            var match = A.Dummy<Match>()
                .With(m => m.League, WorldCupLeague)
                .With(m => m.Teams, new List<Team>
                {
                    A.Dummy<Team>()
                        .With(t => t.Name, "AC Milan")
                        .With(t => t.IsHome, true)
                        .With(t => t.Statistic, new TeamStatistic(0, 2, 1)),
                    A.Dummy<Team>()
                        .With(t => t.Name, "Juventus")
                        .With(t => t.IsHome, false)
                        .With(t => t.Statistic, new TeamStatistic(0, 0, 1)),
                })
                .With(m => m.TimeLines, null);

            timelineService.GetTimelineEvents("sr:match", "eu", Language.en_US)
                .Returns(new Tuple<Match, IEnumerable<TimelineCommentary>>(match, Enumerable.Empty<TimelineCommentary>()));

            // Act
            await fetchTimelineTask.FetchTimelineEvents("sr:match", "eu", Language.en_US);

            // Assert
            await messageBus.DidNotReceive().Publish<IMatchTimelinesFetchedMessage>(Arg.Any<MatchTimelinesFetchedMessage>());
        }

        [Fact]
        public async Task FetchTimelines_TimelinesEmpty_ShouldNotPublishMatchTimelinesFetchedMessage()
        {
            // Arrange
            var match = A.Dummy<Match>()
                .With(m => m.League, WorldCupLeague)
                .With(m => m.Teams, new List<Team>
                {
                    A.Dummy<Team>()
                        .With(t => t.Name, "AC Milan")
                        .With(t => t.IsHome, true)
                        .With(t => t.Statistic, new TeamStatistic(0, 2, 1)),
                    A.Dummy<Team>()
                        .With(t => t.Name, "Juventus")
                        .With(t => t.IsHome, false)
                        .With(t => t.Statistic, new TeamStatistic(0, 0, 1)),
                })
                .With(m => m.TimeLines, new List<TimelineEvent>());

            timelineService.GetTimelineEvents("sr:match", "eu", Language.en_US)
                .Returns(new Tuple<Match, IEnumerable<TimelineCommentary>>(match, Enumerable.Empty<TimelineCommentary>()));

            // Act
            await fetchTimelineTask.FetchTimelineEvents("sr:match", "eu", Language.en_US);

            // Assert
            await messageBus.DidNotReceive().Publish<IMatchTimelinesFetchedMessage>(Arg.Any<MatchTimelinesFetchedMessage>());
        }

        [Fact]
        public async Task FetchTimelines_TimelinesNotEmpty_ShouldPublishMatchTimelinesFetchedMessage()
        {
            // Arrange
            var match = A.Dummy<Match>()
                .With(m => m.League, WorldCupLeague)
                .With(m => m.Teams, new List<Team>
                {
                    A.Dummy<Team>()
                        .With(t => t.Name, "AC Milan")
                        .With(t => t.IsHome, true)
                        .With(t => t.Statistic, new TeamStatistic(0, 2, 1)),
                    A.Dummy<Team>()
                        .With(t => t.Name, "Juventus")
                        .With(t => t.IsHome, false)
                        .With(t => t.Statistic, new TeamStatistic(0, 0, 1)),
                })
                .With(m => m.TimeLines, new List<TimelineEvent>
                {
                    A.Dummy<TimelineEvent>().With(t => t.Type, EventType.MatchStarted)
                });

            timelineService.GetTimelineEvents("sr:match", "eu", Language.en_US)
                .Returns(new Tuple<Match, IEnumerable<TimelineCommentary>>(match, Enumerable.Empty<TimelineCommentary>()));

            // Act
            await fetchTimelineTask.FetchTimelineEvents("sr:match", "eu", Language.en_US);

            // Assert
            await messageBus.Received(1).Publish<IMatchTimelinesFetchedMessage>(Arg.Any<MatchTimelinesFetchedMessage>());
        }

        [Fact]
        public async Task FetchTimelines_CommentariesNull_ShouldNotPublishMatchCommentaryFetchedMessage()
        {
            // Arrange
            var match = A.Dummy<Match>()
                .With(m => m.League, WorldCupLeague)
                .With(m => m.Teams, new List<Team>
                {
                    A.Dummy<Team>()
                        .With(t => t.Name, "AC Milan")
                        .With(t => t.IsHome, true)
                        .With(t => t.Statistic, new TeamStatistic(0, 2, 1)),
                    A.Dummy<Team>()
                        .With(t => t.Name, "Juventus")
                        .With(t => t.IsHome, false)
                        .With(t => t.Statistic, new TeamStatistic(0, 0, 1)),
                });
            timelineService.GetTimelineEvents("sr:match", "eu", Language.en_US)
                .Returns(new Tuple<Match, IEnumerable<TimelineCommentary>>(match, Enumerable.Empty<TimelineCommentary>()));

            // Act
            await fetchTimelineTask.FetchTimelineEvents("sr:match", "eu", Language.en_US);

            // Assert
            await messageBus.DidNotReceive().Publish<IMatchCommentaryFetchedMessage>(Arg.Any<MatchCommentaryFetchedMessage>());
        }

        [Fact]
        public async Task FetchTimelines_CommentariesEmpty_ShouldNotPublishMatchCommentaryFetchedMessage()
        {
            // Arrange
            var match = A.Dummy<Match>()
                .With(m => m.League, WorldCupLeague)
                .With(m => m.Teams, new List<Team>
                {
                    A.Dummy<Team>()
                        .With(t => t.Name, "AC Milan")
                        .With(t => t.IsHome, true)
                        .With(t => t.Statistic, new TeamStatistic(0, 2, 1)),
                    A.Dummy<Team>()
                        .With(t => t.Name, "Juventus")
                        .With(t => t.IsHome, false)
                        .With(t => t.Statistic, new TeamStatistic(0, 0, 1)),
                })
                .With(m => m.TimeLines, new List<TimelineEvent>
                {
                    A.Dummy<TimelineEvent>()
                        .With(t => t.Type, EventType.MatchStarted)
                        .With(t => t.Commentaries, new List<Commentary>())
                });
            timelineService.GetTimelineEvents("sr:match", "eu", Language.en_US)
                .Returns(new Tuple<Match, IEnumerable<TimelineCommentary>>(match, Enumerable.Empty<TimelineCommentary>()));

            // Act
            await fetchTimelineTask.FetchTimelineEvents("sr:match", "eu", Language.en_US);

            // Assert
            await messageBus.DidNotReceive().Publish<IMatchCommentaryFetchedMessage>(Arg.Any<MatchCommentaryFetchedMessage>());
        }

        [Fact]
        public async Task FetchTimelines_CommentariesNotEmpty_ShouldPublishMatchCommentaryFetchedMessage()
        {
            // Arrange
            var match = A.Dummy<Match>()
                .With(m => m.League, WorldCupLeague)
                .With(m => m.Teams, new List<Team>
                {
                    A.Dummy<Team>()
                        .With(t => t.Name, "AC Milan")
                        .With(t => t.IsHome, true)
                        .With(t => t.Statistic, new TeamStatistic(0, 2, 1)),
                    A.Dummy<Team>()
                        .With(t => t.Name, "Juventus")
                        .With(t => t.IsHome, false)
                        .With(t => t.Statistic, new TeamStatistic(0, 0, 1)),
                })
                .With(m => m.TimeLines, new List<TimelineEvent>
                {
                    A.Dummy<TimelineEvent>()
                        .With(t => t.Id, "1")
                        .With(t => t.Type, EventType.MatchStarted),
                    A.Dummy<TimelineEvent>()
                        .With(t => t.Id, "2")
                        .With(t => t.Type, EventType.ScoreChange)
                });
            var commentaries = new List<TimelineCommentary> {
                    new TimelineCommentary (1, new List<Commentary>
                    {
                        new Commentary("match has started")
                    }),
                    new TimelineCommentary (1, new List<Commentary>
                    {
                        new Commentary( "Goal! Cucuta Deportivo FC have got their heads in front thanks to a James Castro strike."),
                        new Commentary("Carmelo Valencia with an assist there.")
                    })};

            timelineService.GetTimelineEvents("sr:match", "eu", Language.en_US)
                .Returns(new Tuple<Match, IEnumerable<TimelineCommentary>>(match, commentaries));

            // Act
            await fetchTimelineTask.FetchTimelineEvents("sr:match", "eu", Language.en_US);

            // Assert
            await messageBus.Received(2).Publish<IMatchCommentaryFetchedMessage>(Arg.Any<MatchCommentaryFetchedMessage>());
        }
    }
}
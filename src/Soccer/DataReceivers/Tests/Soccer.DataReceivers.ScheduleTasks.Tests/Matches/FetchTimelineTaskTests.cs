using System.Collections.Generic;
using System.Threading.Tasks;
using MassTransit;
using NSubstitute;
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
            timelineService.GetTimelines("sr:match", "eu", Language.en_US).Returns(new Match
            {
                Teams = new List<Team>(),
                Referee = "AAA"
            });

            // Act
            await fetchTimelineTask.FetchTimelines("sr:match", "eu", Language.en_US);

            // Assert
            await messageBus.Received(1).Publish<IMatchUpdatedConditionsMessage>(Arg.Is<MatchUpdatedConditionsMessage>(m => m.Referee == "AAA"));
        }

        [Fact]
        public async Task FetchTimelines_HasAttendance_ShouldPublishMatchUpdatedConditionsMessage()
        {
            // Arrange
            timelineService.GetTimelines("sr:match", "eu", Language.en_US).Returns(new Match
            {
                Teams = new List<Team>(),
                Attendance = 10000
            });

            // Act
            await fetchTimelineTask.FetchTimelines("sr:match", "eu", Language.en_US);

            // Assert
            await messageBus.Received(1).Publish<IMatchUpdatedConditionsMessage>(Arg.Is<MatchUpdatedConditionsMessage>(m => m.Attendance == 10000));
        }

        [Fact]
        public async Task FetchTimelines_HasBothAttendanceAndReferee_ShouldPublishMatchUpdatedConditionsMessage()
        {
            // Arrange
            timelineService.GetTimelines("sr:match", "eu", Language.en_US).Returns(new Match
            {
                Teams = new List<Team>(),
                Referee = "AAA",
                Attendance = 10000
            });

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
            timelineService.GetTimelines("sr:match", "eu", Language.en_US).Returns(new Match
            {
                Teams = new List<Team>()
            });

            // Act
            await fetchTimelineTask.FetchTimelines("sr:match", "eu", Language.en_US);

            // Assert
            await messageBus.DidNotReceive().Publish<IMatchUpdatedConditionsMessage>(Arg.Any<MatchUpdatedConditionsMessage>());
        }

        [Fact]
        public async Task FetchTimelines_TeamStatisticNull_ShouldNotPublishTeamStatisticMessage()
        {
            // Arrange
            timelineService.GetTimelines("sr:match", "eu", Language.en_US).Returns(new Match
            {
                Teams = new List<Team>
                {
                    new Team{ IsHome = true, Name ="AC Milan" },
                    new Team{ IsHome = false, Name ="Juventus" }
                }
            });

            // Act
            await fetchTimelineTask.FetchTimelines("sr:match", "eu", Language.en_US);

            // Assert
            await messageBus.DidNotReceive().Publish<ITeamStatisticUpdatedMessage>(Arg.Any<TeamStatisticUpdatedMessage>());
        }

        [Fact]
        public async Task FetchTimelines_BothTeamStatisticNotNull_ShouldPublishTeamStatisticMessage()
        {
            // Arrange
            timelineService.GetTimelines("sr:match", "eu", Language.en_US).Returns(new Match
            {
                Teams = new List<Team>
                {
                    new Team{ IsHome = true, Name ="AC Milan", Statistic = new TeamStatistic(0, 2) },
                    new Team{ IsHome = false, Name ="Juventus", Statistic = new TeamStatistic(0, 0) }
                }
            });

            // Act
            await fetchTimelineTask.FetchTimelines("sr:match", "eu", Language.en_US);

            // Assert
            await messageBus.Received(2).Publish<ITeamStatisticUpdatedMessage>(Arg.Any<TeamStatisticUpdatedMessage>());
        }

        [Fact]
        public async Task FetchTimelines_TeamStatisticNotNull_ShouldPublishTeamStatisticMessage()
        {
            // Arrange
            timelineService.GetTimelines("sr:match", "eu", Language.en_US).Returns(new Match
            {
                Teams = new List<Team>
                {
                    new Team{ IsHome = true, Name ="AC Milan", Statistic = new TeamStatistic(0, 2) },
                    new Team{ IsHome = false, Name ="Juventus" }
                }
            });

            // Act
            await fetchTimelineTask.FetchTimelines("sr:match", "eu", Language.en_US);

            // Assert
            await messageBus.Received(1).Publish<ITeamStatisticUpdatedMessage>(Arg.Any<TeamStatisticUpdatedMessage>());
        }

        [Fact]
        public async Task FetchTimelines_TimelinesNull_ShouldNotPublishMatchTimelinesFetchedMessage()
        {
            // Arrange
            timelineService.GetTimelines("sr:match", "eu", Language.en_US).Returns(new Match
            {
                Teams = new List<Team>
                {
                    new Team{ IsHome = true, Name ="AC Milan", Statistic = new TeamStatistic(0, 2) },
                    new Team{ IsHome = false, Name ="Juventus", Statistic = new TeamStatistic(0, 0) }
                }
            });

            // Act
            await fetchTimelineTask.FetchTimelines("sr:match", "eu", Language.en_US);

            // Assert
            await messageBus.DidNotReceive().Publish<IMatchTimelinesFetchedMessage>(Arg.Any<MatchTimelinesFetchedMessage>());
        }

        [Fact]
        public async Task FetchTimelines_TimelinesEmpty_ShouldNotPublishMatchTimelinesFetchedMessage()
        {
            // Arrange
            timelineService.GetTimelines("sr:match", "eu", Language.en_US).Returns(new Match
            {
                Teams = new List<Team>
                {
                    new Team{ IsHome = true, Name ="AC Milan", Statistic = new TeamStatistic(0, 2) },
                    new Team{ IsHome = false, Name ="Juventus", Statistic = new TeamStatistic(0, 0) }
                },
                TimeLines = new List<TimelineEvent>()
            });

            // Act
            await fetchTimelineTask.FetchTimelines("sr:match", "eu", Language.en_US);

            // Assert
            await messageBus.DidNotReceive().Publish<IMatchTimelinesFetchedMessage>(Arg.Any<MatchTimelinesFetchedMessage>());
        }

        [Fact]
        public async Task FetchTimelines_TimelinesNotEmpty_ShouldPublishMatchTimelinesFetchedMessage()
        {
            // Arrange
            timelineService.GetTimelines("sr:match", "eu", Language.en_US).Returns(new Match
            {
                Teams = new List<Team>
                {
                    new Team{ IsHome = true, Name ="AC Milan", Statistic = new TeamStatistic(0, 2) },
                    new Team{ IsHome = false, Name ="Juventus", Statistic = new TeamStatistic(0, 0) }
                },
                TimeLines = new List<TimelineEvent>
                {
                    new TimelineEvent{ Type = EventType.MatchStarted }
                }
            });

            // Act
            await fetchTimelineTask.FetchTimelines("sr:match", "eu", Language.en_US);

            // Assert
            await messageBus.Received(1).Publish<IMatchTimelinesFetchedMessage>(Arg.Any<MatchTimelinesFetchedMessage>());
        }

        [Fact]
        public async Task FetchTimelines_CommentariesNull_ShouldNotPublishMatchCommentaryFetchedMessage()
        {
            // Arrange
            timelineService.GetTimelines("sr:match", "eu", Language.en_US).Returns(new Match
            {
                Teams = new List<Team>
                {
                    new Team{ IsHome = true, Name ="AC Milan", Statistic = new TeamStatistic(0, 2) },
                    new Team{ IsHome = false, Name ="Juventus", Statistic = new TeamStatistic(0, 0) }
                }
            });

            // Act
            await fetchTimelineTask.FetchTimelines("sr:match", "eu", Language.en_US);

            // Assert
            await messageBus.DidNotReceive().Publish<IMatchCommentaryFetchedMessage>(Arg.Any<MatchCommentaryFetchedMessage>());
        }

        [Fact]
        public async Task FetchTimelines_CommentariesEmpty_ShouldNotPublishMatchCommentaryFetchedMessage()
        {
            // Arrange
            timelineService.GetTimelines("sr:match", "eu", Language.en_US).Returns(new Match
            {
                Teams = new List<Team>
                {
                    new Team{ IsHome = true, Name ="AC Milan", Statistic = new TeamStatistic(0, 2) },
                    new Team{ IsHome = false, Name ="Juventus", Statistic = new TeamStatistic(0, 0) }
                },
                TimeLines = new List<TimelineEvent>
                {
                    new TimelineEvent{ Type = EventType.MatchStarted, Commentaries = new List<Commentary>() }
                }
            });

            // Act
            await fetchTimelineTask.FetchTimelines("sr:match", "eu", Language.en_US);

            // Assert
            await messageBus.DidNotReceive().Publish<IMatchCommentaryFetchedMessage>(Arg.Any<MatchCommentaryFetchedMessage>());
        }

        [Fact]
        public async Task FetchTimelines_CommentariesNotEmpty_ShouldPublishMatchCommentaryFetchedMessage()
        {
            // Arrange
            timelineService.GetTimelines("sr:match", "eu", Language.en_US).Returns(new Match
            {
                League = new League { Id = "sr:league" },
                Teams = new List<Team>
                {
                    new Team{ IsHome = true, Name ="AC Milan", Statistic = new TeamStatistic(0, 2) },
                    new Team{ IsHome = false, Name ="Juventus", Statistic = new TeamStatistic(0, 0) }
                },
                TimeLines = new List<TimelineEvent>
                {
                    new TimelineEvent{ Id = "1", Type = EventType.MatchStarted},
                    new TimelineEvent{ Id = "2", Type = EventType.ScoreChange}
                },
                TimelineCommentaries = new List<TimelineCommentary> {
                    new TimelineCommentary (1, new List<Commentary>
                        {
                            new Commentary{ Text = "match has started"}
                        }),
                    new TimelineCommentary (1, new List<Commentary>
                        {
                            new Commentary{ Text = "Goal! Cucuta Deportivo FC have got their heads in front thanks to a James Castro strike."},
                            new Commentary{ Text = "Carmelo Valencia with an assist there."}
                        })
                }
            });

            // Act
            await fetchTimelineTask.FetchTimelines("sr:match", "eu", Language.en_US);

            // Assert
            await messageBus.Received(2).Publish<IMatchCommentaryFetchedMessage>(Arg.Any<MatchCommentaryFetchedMessage>());
        }
    }
}

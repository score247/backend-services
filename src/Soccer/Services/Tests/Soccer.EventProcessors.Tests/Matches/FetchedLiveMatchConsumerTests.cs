using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Fanex.Data.Repository;
using Fanex.Logging;
using MassTransit;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Score247.Shared.Tests;
using Soccer.Core.Matches.Events;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.Database.Matches.Commands;
using Soccer.Database.Matches.Criteria;
using Soccer.EventProcessors.Matches;
using Soccer.EventProcessors.Matches.Filters;
using Xunit;

namespace Soccer.EventProcessors.Tests.Matches
{
    [Trait("Soccer.EventProcessors", "FetchedLiveMatchConsumer")]
    public class FetchedLiveMatchConsumerTests
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly ILogger logger;
        private readonly FetchedLiveMatchConsumer fetchedLiveMatchConsumer;
        private readonly ConsumeContext<ILiveMatchFetchedMessage> context;
#pragma warning disable S2699 // Tests should include assertions

        public FetchedLiveMatchConsumerTests()
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            logger = Substitute.For<ILogger>();
            context = Substitute.For<ConsumeContext<ILiveMatchFetchedMessage>>();
            var messageBus = Substitute.For<IBus>();
            var liveMatchFilter = new LiveMatchFilter(new LiveMatchRangeValidator());

            fetchedLiveMatchConsumer = new FetchedLiveMatchConsumer(messageBus, dynamicRepository, liveMatchFilter, logger);
        }

        [Fact]
        public async Task Consume_LanguageIsNull_ShouldNotFetchLiveMatchFromRepository()
        {
            await fetchedLiveMatchConsumer.Consume(context);

            await dynamicRepository.DidNotReceive().FetchAsync<Match>(Arg.Any<GetLiveMatchesCriteria>());
        }

        [Fact]
        public async Task Consume_ShouldFetchLiveMatchFromRepository()
        {
            context.Message
                .Returns(new LiveMatchFetchedMessage(Language.en_US, Enumerable.Empty<Match>()));

            await fetchedLiveMatchConsumer.Consume(context);

            await dynamicRepository.Received(1).FetchAsync<Match>(Arg.Any<GetLiveMatchesCriteria>());
        }

        [Fact]
        public async Task Consume_HasNewMatches_ShouldExecuteCommand()
        {
            var match = A.Dummy<Match>()
                .With(m => m.Id, "match:not:started")
                .With(m => m.MatchResult, A.Dummy<MatchResult>().With(r => r.EventStatus, MatchStatus.Live))
                ;
            context.Message
                .Returns(new LiveMatchFetchedMessage(Language.en_US, new List<Match> { match }));

            await fetchedLiveMatchConsumer.Consume(context);

            await dynamicRepository.Received(1).ExecuteAsync(Arg.Is<InsertOrRemoveLiveMatchesCommand>(
                cmd => cmd.NewMatches.Contains("match:not:started")
                && cmd.RemovedMatchIds.Equals("[]")));
        }

        [Fact]
        public async Task Consume_RemoveMatches_ShouldExecuteCommand()
        {
            var match = A.Dummy<Match>()
                .With(m => m.Id, "match:closed")
                .With(m => m.MatchResult, A.Dummy<MatchResult>().With(r => r.EventStatus, MatchStatus.Live))
                ;
            context.Message
                .Returns(new LiveMatchFetchedMessage(Language.en_US, Enumerable.Empty<Match>()));

            dynamicRepository.FetchAsync<Match>(Arg.Any<GetLiveMatchesCriteria>()).Returns(new List<Match> { match });

            await fetchedLiveMatchConsumer.Consume(context);

            await dynamicRepository.Received(1).ExecuteAsync(Arg.Is<InsertOrRemoveLiveMatchesCommand>(
                cmd => cmd.RemovedMatchIds.Contains("match:closed")
                        && cmd.NewMatches.Equals("[]")));
        }

        [Fact]
        public async Task Consume_HasNewMatchesButOutOfRange_ShouldNotInsert()
        {
            // Arrange
            var matchesFromApi = new List<Match> { StubNotStartedMatch("match:not:started", DateTimeOffset.Now.AddMinutes(11)) };

            context.Message
                .Returns(new LiveMatchFetchedMessage(Language.en_US, matchesFromApi));

            dynamicRepository.FetchAsync<Match>(Arg.Any<GetLiveMatchesCriteria>())
                .Returns(new List<Match>
                {
                    StubNotStartedMatch("match:not:started", DateTimeOffset.Now.AddMinutes(11))
                });

            // Act
            await fetchedLiveMatchConsumer.Consume(context);

            // Assert
            await dynamicRepository.Received(1).ExecuteAsync(Arg.Is<InsertOrRemoveLiveMatchesCommand>(
                cmd => cmd.RemovedMatchIds.Contains("match:not:started")
                        && cmd.NewMatches.Equals("[]")));
        }

        [Fact]
        public async Task Consume_HasBothNewAndClosedMatchInRange_ShouldExecuteCommand()
        {
            // Arrange
            var newMatch = StubNotStartedMatch("match:not:started", DateTimeOffset.Now.AddMinutes(3));
            var liveMatch = StubLiveMatch("match:live");
            var closedMatch = StubClosedMatch("match:closed", endedTime: null);
            var matchesFromApi = new List<Match> { newMatch, liveMatch, closedMatch };

            context.Message
                .Returns(new LiveMatchFetchedMessage(Language.en_US, matchesFromApi));

            dynamicRepository.FetchAsync<Match>(Arg.Any<GetLiveMatchesCriteria>())
                .Returns(new List<Match>
                {
                    StubLiveMatch("match:live"),
                    StubClosedMatch("match:closed", endedTime: DateTimeOffset.Now.AddMinutes(-5))
                });

            // Act
            await fetchedLiveMatchConsumer.Consume(context);

            // Assert
            await dynamicRepository.Received(1).ExecuteAsync(Arg.Is<InsertOrRemoveLiveMatchesCommand>(
                cmd => !cmd.RemovedMatchIds.Contains("match:closed")
                        && cmd.NewMatches.Contains("match:not:started")));
        }

        [Fact]
        public async Task Consume_HasNewAndClosedMatchOutOfRange_ShouldExecuteCommand()
        {
            // Arrange
            var newMatch = StubNotStartedMatch("match:not:started", DateTimeOffset.Now.AddMinutes(3));
            var liveMatch = StubLiveMatch("match:live");
            var closedMatch = StubClosedMatch("match:closed", endedTime: null);
            var matchesFromApi = new List<Match> { newMatch, liveMatch, closedMatch };

            context.Message
                .Returns(new LiveMatchFetchedMessage(Language.en_US, matchesFromApi));

            dynamicRepository.FetchAsync<Match>(Arg.Any<GetLiveMatchesCriteria>())
                .Returns(new List<Match>
                {
                    StubLiveMatch("match:live"),
                    StubClosedMatch("match:closed", endedTime: DateTimeOffset.Now.AddMinutes(-11))
                });

            // Act
            await fetchedLiveMatchConsumer.Consume(context);

            // Assert
            await dynamicRepository.Received(1).ExecuteAsync(Arg.Is<InsertOrRemoveLiveMatchesCommand>(
                cmd => cmd.RemovedMatchIds.Contains("match:closed")
                        && cmd.NewMatches.Contains("match:not:started")));
        }

        [Fact]
        public async Task Consume_ClosedMatchOutOfRange_ShouldExecuteCommand()
        {
            // Arrange
            var closedMatch = StubClosedMatch("match:closed", endedTime: null);
            var matchesFromApi = new List<Match> { closedMatch };

            context.Message
                .Returns(new LiveMatchFetchedMessage(Language.en_US, matchesFromApi));

            dynamicRepository.FetchAsync<Match>(Arg.Any<GetLiveMatchesCriteria>())
                .Returns(new List<Match>
                {
                    StubClosedMatch("match:closed", endedTime: DateTimeOffset.Now.AddMinutes(-11))
                });

            // Act
            await fetchedLiveMatchConsumer.Consume(context);

            // Assert
            await dynamicRepository.Received(1).ExecuteAsync(Arg.Is<InsertOrRemoveLiveMatchesCommand>(
                cmd => cmd.RemovedMatchIds.Contains("match:closed")
                        && cmd.NewMatches.Equals("[]")));
        }

        [Fact]
        public async Task Consume_LiveMatchedNotChanged_ShouldNotExecuteCommand()
        {
            var newMatch = A.Dummy<Match>()
                .With(m => m.Id, "1")
                .With(m => m.MatchResult, A.Dummy<MatchResult>().With(r => r.EventStatus, MatchStatus.Live))
                ;
            context.Message
                .Returns(new LiveMatchFetchedMessage(Language.en_US, new List<Match> { newMatch }));

            dynamicRepository.FetchAsync<Match>(Arg.Any<GetLiveMatchesCriteria>())
                .Returns(new List<Match> { newMatch });

            await fetchedLiveMatchConsumer.Consume(context);

            await dynamicRepository.DidNotReceive().ExecuteAsync(Arg.Any<InsertOrRemoveLiveMatchesCommand>());
        }

        [Fact]
        public async Task Consume_ExecuteCommandThrowsException_ShouldLogError()
        {
            var match = A.Dummy<Match>()
                .With(m => m.Id, "1")
                .With(m => m.MatchResult, A.Dummy<MatchResult>().With(r => r.EventStatus, MatchStatus.Live))
                ;
            context.Message
                .Returns(new LiveMatchFetchedMessage(Language.en_US, Enumerable.Empty<Match>()));

            dynamicRepository.FetchAsync<Match>(Arg.Any<GetLiveMatchesCriteria>()).Returns(new List<Match> { match });

            dynamicRepository.ExecuteAsync(Arg.Any<InsertOrRemoveLiveMatchesCommand>()).Throws(new InvalidOperationException("Invalid"));

            await fetchedLiveMatchConsumer.Consume(context);

            await logger.Received(1).ErrorAsync(Arg.Any<string>(), Arg.Any<Exception>());
        }

        private static Match StubNotStartedMatch(string id, DateTimeOffset eventDate)
            => A.Dummy<Match>()
                .With(m => m.Id, id)
                .With(m => m.EventDate, eventDate)
                .With(m => m.MatchResult, A.Dummy<MatchResult>().With(r => r.EventStatus, MatchStatus.NotStarted))
                ;

        private static Match StubLiveMatch(string id)
            => A.Dummy<Match>()
                .With(m => m.Id, id)
                .With(m => m.MatchResult, A.Dummy<MatchResult>().With(r => r.EventStatus, MatchStatus.Live))
                ;

        private static Match StubClosedMatch(string id, DateTimeOffset? endedTime)
            => A.Dummy<Match>()
                .With(m => m.Id, id)
                .With(m => m.MatchResult, A.Dummy<MatchResult>()
                    .With(r => r.EventStatus, MatchStatus.Closed)
                    .With(r => r.MatchStatus, MatchStatus.Ended)
                    )
                .With(m => m.LatestTimeline, endedTime == null
                    ? null
                    : A.Dummy<TimelineEvent>()
                        .With(t => t.Type, EventType.MatchEnded)
                        .With(t => t.Time, (DateTimeOffset)endedTime));
    }

#pragma warning restore S2699 // Tests should include assertions
}
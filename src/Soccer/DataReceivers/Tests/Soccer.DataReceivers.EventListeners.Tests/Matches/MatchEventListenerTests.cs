using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using Fanex.Logging;
using MassTransit;
using NSubstitute;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Matches.Models;
using Soccer.Core.Matches.QueueMessages;
using Soccer.Core.Shared.Enumerations;
using Soccer.DataProviders.Leagues;
using Soccer.DataProviders.Matches.Services;
using Soccer.DataReceivers.EventListeners.Matches;
using Soccer.DataReceivers.Odds;
using Xunit;

namespace Soccer.DataReceivers.EventListeners.Tests.Matches
{
    public class MatchEventListenerTests
    {
        private readonly IBus messageBus;
        private readonly ILeagueService internalLeagueService;
        private readonly IMatchEventListenerService eventListenerService;
        private readonly ILogger logger;
        private readonly MatchEventListener matchEventListener;

        public MatchEventListenerTests()
        {
            messageBus = Substitute.For<IBus>();
            internalLeagueService = Substitute.For<ILeagueService>();
            logger = Substitute.For<ILogger>();
            eventListenerService = Substitute.For<IMatchEventListenerService>();
            matchEventListener = new MatchEventListener(messageBus, eventListenerService, logger, internalLeagueService);

            internalLeagueService.GetLeagues(Language.en_US).Returns(new List<League> {
                new League("1", "A"),
                new League("2", "A")
            });
        }

        [Fact]
        public async Task HandleEvent_NotInMajorLeagues_NotPublishMessages()
        {
            // Arrange
            await matchEventListener.StartAsync(default);
            var matchEvent = new MatchEvent("3", "1", A.Dummy<MatchResult>(), A.Dummy<TimelineEvent>());

            // Act
            await matchEventListener.HandleEvent(matchEvent, default);

            // Assert
            await messageBus.DidNotReceive().Publish<IMatchEventReceivedMessage>(Arg.Any<MatchEventReceivedMessage>(), default);
        }

        [Fact]
        public async Task HandleEvent_InMajorLeagues_PublishMessages()
        {
            // Arrange
            await matchEventListener.StartAsync(default);
            var matchEvent = new MatchEvent("2", "1", A.Dummy<MatchResult>(), A.Dummy<TimelineEvent>());

            // Act
            await matchEventListener.HandleEvent(matchEvent, default);

            // Assert
            await messageBus.Received().Publish<IMatchEventReceivedMessage>(Arg.Any<MatchEventReceivedMessage>(), default);
        }
    }
}
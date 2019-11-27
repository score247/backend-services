using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using MassTransit;
using NSubstitute;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.Core.Timelines.Models;
using Soccer.Core.Timelines.QueueMessages;
using Soccer.Database.Timelines.Commands;
using Soccer.EventProcessors.Timeline;
using Xunit;

namespace Soccer.EventProcessors.Tests.Timelines
{
#pragma warning disable S2699 // Tests should include assertions

    [Trait("Soccer.EventProcessors", "FetchCommentaryConsumer")]
    public class FetchCommentaryConsumerTests
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly ConsumeContext<IMatchCommentaryFetchedMessage> context;

        private readonly FetchCommentaryConsumer consumer;

        public FetchCommentaryConsumerTests()
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            context = Substitute.For<ConsumeContext<IMatchCommentaryFetchedMessage>>();

            consumer = new FetchCommentaryConsumer(dynamicRepository);
        }

        [Fact]
        public async Task Consume_InvalidMessage_ShouldNotExecuteCommand()
        {
            await consumer.Consume(context);

            await dynamicRepository.DidNotReceive().ExecuteAsync(Arg.Any<InsertCommentaryCommand>());
        }

        [Fact]
        public async Task Consume_ValidMessage_ShouldExecuteCommand()
        {
            context.Message.Returns(new MatchCommentaryFetchedMessage(
                "sr:league:1",
                "sr:match:1",
                new TimelineCommentary(1, new List<Commentary>()),
                Language.en_US));

            await consumer.Consume(context);

            await dynamicRepository.Received(1).ExecuteAsync(Arg.Is<InsertCommentaryCommand>(m => m.MatchId == "sr:match:1"));
        }
    }

#pragma warning restore S2699 // Tests should include assertions
}
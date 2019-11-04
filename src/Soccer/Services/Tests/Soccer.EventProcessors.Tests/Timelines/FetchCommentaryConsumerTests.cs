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
using Soccer.EventProcessors.Leagues.Filters;
using Soccer.EventProcessors.Timeline;
using Xunit;

namespace Soccer.EventProcessors.Tests.Timelines
{
    [Trait("Soccer.EventProcessors", "FetchCommentaryConsumer")]
    public class FetchCommentaryConsumerTests
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly IMajorLeagueFilter<string, bool> majorLeagueFilter;
        private readonly ConsumeContext<IMatchCommentaryFetchedMessage> context;

        private readonly FetchCommentaryConsumer consumer;

        public FetchCommentaryConsumerTests()
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            majorLeagueFilter = Substitute.For<IMajorLeagueFilter<string, bool>>();
            context = Substitute.For<ConsumeContext<IMatchCommentaryFetchedMessage>>();

            consumer = new FetchCommentaryConsumer(dynamicRepository, majorLeagueFilter);
        }

        [Fact]
        public async Task Consume_InvalidMessage_ShouldNotExecuteCommand()
        {
            await consumer.Consume(context);

            await dynamicRepository.DidNotReceive().ExecuteAsync(Arg.Any<InsertCommentaryCommand>());
        }

        [Fact]
        public async Task Consume_NotInMajorLeagues_ShouldNotExecuteCommand()
        {
            majorLeagueFilter.Filter(Arg.Is<string>(leagueId => leagueId == "sr:league:1")).Returns(true);

            context.Message.Returns(new MatchCommentaryFetchedMessage(
                "sr:league:2",
                "sr:match:1",
                new TimelineCommentary(1, new List<Commentary>()),
                Language.en_US));

            await consumer.Consume(context);

            await dynamicRepository.DidNotReceive().ExecuteAsync(Arg.Is<InsertCommentaryCommand>(m => m.MatchId == "sr:match:1"));
        }

        [Fact]
        public async Task Consume_ValidMessage_ShouldExecuteCommand()
        {
            majorLeagueFilter.Filter(Arg.Is<string>(leagueId => leagueId == "sr:league:1")).Returns(true);

            context.Message.Returns(new MatchCommentaryFetchedMessage(
                "sr:league:1",
                "sr:match:1",
                new TimelineCommentary(1, new List<Commentary>()),
                Language.en_US));

            await consumer.Consume(context);

            await dynamicRepository.Received(1).ExecuteAsync(Arg.Is<InsertCommentaryCommand>(m => m.MatchId == "sr:match:1"));
        }
    }
}
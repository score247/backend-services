using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using Fanex.Logging;
using MassTransit;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Soccer.Core.Matches.Events;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.Database.Matches.Commands;
using Soccer.Database.Matches.Criteria;
using Soccer.EventProcessors._Shared.Filters;
using Soccer.EventProcessors.Leagues;
using Soccer.EventProcessors.Matches;
using Xunit;

namespace Soccer.EventProcessors.Tests.Matches
{
    [Trait("Soccer.EventProcessors", "FetchedLiveMatchConsumer")]
    public class FetchedLiveMatchConsumerTests
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly IBus messageBus;
        private readonly IAsyncFilter<IEnumerable<Match>, IEnumerable<Match>> leagueFilter;
        private readonly IFilter<IEnumerable<Match>, IEnumerable<Match>> eventDateFilter;
        private readonly ILeagueGenerator leagueGenerator;
        private readonly ILogger logger;

        private readonly FetchedLiveMatchConsumer fetchedLiveMatchConsumer;
        private readonly ConsumeContext<ILiveMatchFetchedMessage> context;

        public FetchedLiveMatchConsumerTests()
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            messageBus = Substitute.For<IBus>();
            leagueFilter = Substitute.For<IAsyncFilter<IEnumerable<Match>, IEnumerable<Match>>>();
            eventDateFilter = Substitute.For<IFilter<IEnumerable<Match>, IEnumerable<Match>>>();
            leagueGenerator = Substitute.For<ILeagueGenerator>();
            logger = Substitute.For<ILogger>();

            context = Substitute.For<ConsumeContext<ILiveMatchFetchedMessage>>();

            fetchedLiveMatchConsumer = new FetchedLiveMatchConsumer(messageBus, dynamicRepository, leagueFilter, eventDateFilter, leagueGenerator, logger);
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
            context.Message.Returns(new LiveMatchFetchedMessage(Language.en_US, Enumerable.Empty<Match>()));

            await fetchedLiveMatchConsumer.Consume(context);

            await dynamicRepository.Received(1).FetchAsync<Match>(Arg.Any<GetLiveMatchesCriteria>());
        }

        [Fact]
        public async Task Consume_HasNewMatches_ShouldExecuteCommand()
        {
            var match = new Match { Id = "1", MatchResult = new MatchResult { EventStatus = MatchStatus.Live } };
            context.Message
                .Returns(new LiveMatchFetchedMessage(Language.en_US, new List<Match> { match }));

            leagueFilter.Filter(Arg.Any<IEnumerable<Match>>())
                .Returns(new List<Match> { match });

            leagueGenerator.GenerateInternationalCode(Arg.Is<Match>(m => m.Id == "1"))
                .Returns(match);

            eventDateFilter.Filter(Arg.Any<IEnumerable<Match>>())
                .Returns(new List<Match> { match });

            await fetchedLiveMatchConsumer.Consume(context);

            await dynamicRepository.Received(1).ExecuteAsync(Arg.Is<InsertOrRemoveLiveMatchesCommand>(cmd => !cmd.NewMatches.Equals("[]") && cmd.RemovedMatchIds.Equals("[]")));
        }

        [Fact]
        public async Task Consume_RemoveMatches_ShouldExecuteCommand()
        {
            var match = new Match { Id = "1", MatchResult = new MatchResult { EventStatus = MatchStatus.Live } };
            context.Message
                .Returns(new LiveMatchFetchedMessage(Language.en_US, Enumerable.Empty<Match>()));

            dynamicRepository.FetchAsync<Match>(Arg.Any<GetLiveMatchesCriteria>()).Returns(new List<Match> { match });

            await fetchedLiveMatchConsumer.Consume(context);

            await dynamicRepository.Received(1).ExecuteAsync(Arg.Is<InsertOrRemoveLiveMatchesCommand>(cmd => !cmd.RemovedMatchIds.Equals("[]") && cmd.NewMatches.Equals("[]")));
        }

        [Fact]
        public async Task Consume_HasBothNewAndRemovedMatches_ShouldExecuteCommand()
        {
            var newMatch = new Match { Id = "2", MatchResult = new MatchResult { EventStatus = MatchStatus.Live } };
            context.Message
                .Returns(new LiveMatchFetchedMessage(Language.en_US, new List<Match> { newMatch }));

            leagueFilter.Filter(Arg.Any<IEnumerable<Match>>())
                .Returns(new List<Match> { newMatch });

            leagueGenerator.GenerateInternationalCode(Arg.Is<Match>(m => m.Id == "2"))
                .Returns(newMatch);

            eventDateFilter.Filter(Arg.Any<IEnumerable<Match>>())
                .Returns(new List<Match> { newMatch });

            dynamicRepository.FetchAsync<Match>(Arg.Any<GetLiveMatchesCriteria>())
                .Returns(new List<Match> { new Match { Id = "1", MatchResult = new MatchResult { EventStatus = MatchStatus.Live } } });

            await fetchedLiveMatchConsumer.Consume(context);

            await dynamicRepository.Received(1).ExecuteAsync(Arg.Is<InsertOrRemoveLiveMatchesCommand>(cmd => !cmd.RemovedMatchIds.Equals("[]") && !cmd.NewMatches.Equals("[]")));
        }

        [Fact]
        public async Task Consume_LiveMatchedNotChanged_ShouldNotExecuteCommand()
        {
            var newMatch = new Match { Id = "1", MatchResult = new MatchResult { EventStatus = MatchStatus.Live } };
            context.Message
                .Returns(new LiveMatchFetchedMessage(Language.en_US, new List<Match> { newMatch }));

            leagueFilter.Filter(Arg.Any<IEnumerable<Match>>())
                .Returns(new List<Match> { newMatch });

            leagueGenerator.GenerateInternationalCode(Arg.Is<Match>(m => m.Id == "1"))
                .Returns(newMatch);

            eventDateFilter.Filter(Arg.Any<IEnumerable<Match>>())
                .Returns(new List<Match> { newMatch });

            dynamicRepository.FetchAsync<Match>(Arg.Any<GetLiveMatchesCriteria>())
                .Returns(new List<Match> { new Match { Id = "1", MatchResult = new MatchResult { EventStatus = MatchStatus.Live } } });

            await fetchedLiveMatchConsumer.Consume(context);

            await dynamicRepository.DidNotReceive().ExecuteAsync(Arg.Any<InsertOrRemoveLiveMatchesCommand>());
        }

        [Fact]
        public async Task Consume_ExecuteCommandThrowsException_ShouldLogError()
        {
            var match = new Match { Id = "1", MatchResult = new MatchResult { EventStatus = MatchStatus.Live } };
            context.Message
                .Returns(new LiveMatchFetchedMessage(Language.en_US, Enumerable.Empty<Match>()));

            dynamicRepository.FetchAsync<Match>(Arg.Any<GetLiveMatchesCriteria>()).Returns(new List<Match> { match });

            dynamicRepository.ExecuteAsync(Arg.Any<InsertOrRemoveLiveMatchesCommand>()).Throws(new InvalidOperationException("Invalid"));

            await fetchedLiveMatchConsumer.Consume(context);

            await logger.Received(1).ErrorAsync(Arg.Any<string>(), Arg.Any<Exception>());
        }
    }
}

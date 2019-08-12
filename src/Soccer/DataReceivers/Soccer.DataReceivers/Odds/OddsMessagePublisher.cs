namespace Soccer.DataReceivers.Odds
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using MassTransit;
    using Soccer.Core.Matches.QueueMessages.MatchEvents;
    using Soccer.Core.Odds.Messages;
    using Soccer.Core.Odds.Models;
    using Soccer.DataProviders.Odds;

    public interface IOddsMessagePublisher
    {
        Task PublishOdds(IEnumerable<MatchOdds> oddsList, int batchSize);

        Task PublishOdds(INormalEventReceivedMessage message);
    }

    public class OddsMessagePublisher : IOddsMessagePublisher
    {
        private readonly IOddsService oddsService;
        private readonly IBus messageBus;

        public OddsMessagePublisher(
            IOddsService oddsService,
            IBus messageBus)
        {
            this.oddsService = oddsService;
            this.messageBus = messageBus;
        }

        public async Task PublishOdds(IEnumerable<MatchOdds> oddsList, int batchSize)
        {
            var total = oddsList.Count();

            for (var batchIndex = 0; batchIndex * batchSize < total; batchIndex++)
            {
                var oddsBatch = oddsList.Skip(batchIndex * batchSize).Take(batchSize);

                await messageBus.Publish<IOddsChangeMessage>(new OddsChangeMessage(oddsBatch));
            }
        }

        public async Task PublishOdds(INormalEventReceivedMessage message)
        {
            var currentOdds = await oddsService.GetOdds(message.MatchEvent.MatchId, message.MatchEvent.Timeline.Time);

            await messageBus.Publish<IOddsChangeMessage>(
                new OddsChangeMessage(
                    new List<MatchOdds>
                    {
                        currentOdds
                    },
                    message.MatchEvent));
        }
    }
}
﻿namespace Soccer.DataReceivers.Odds
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using MassTransit;
    using Newtonsoft.Json;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Odds;
    using Soccer.Core.Odds.Messages;
    using Soccer.Core.Odds.Models;
    using Soccer.DataProviders.Odds;

    public interface IOddsMessagePublisher
    {
        Task PublishOdds(IEnumerable<MatchOdds> oddsList, int batchSize);

        Task PublishOdds(MatchEvent matchEvent);
    }

    public class OddsMessagePublisher : IOddsMessagePublisher
    {
        private readonly IOddsService oddsService;
        private readonly IBus messageBus;
        private readonly ILogger logger;

        public OddsMessagePublisher(
            IOddsService oddsService,
            IBus messageBus,
            ILogger logger)
        {
            this.oddsService = oddsService;
            this.messageBus = messageBus;
            this.logger = logger;
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

        public async Task PublishOdds(MatchEvent matchEvent)
        {
            try
            {
                if (OddsMovementProcessor.IsTimelineNeedMapWithOddsData(matchEvent.Timeline))
                {
                    var currentOdds = await oddsService.GetOdds(matchEvent.MatchId, matchEvent.Timeline.Time);

                    await messageBus.Publish<IOddsChangeMessage>(
                        new OddsChangeMessage(
                            new List<MatchOdds>
                            {
                        currentOdds
                            },
                            matchEvent));
                }
            }
            catch (Exception ex)
            {
                await logger.ErrorAsync(
                            string.Join(
                            "\r\n",
                            $"Match Event: {JsonConvert.SerializeObject(matchEvent)}",
                            $"Exception: {ex}"),
                            ex);
            }
        }
    }
}
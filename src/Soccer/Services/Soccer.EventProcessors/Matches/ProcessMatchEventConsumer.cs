﻿namespace Soccer.EventProcessors.Matches
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using MassTransit;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Matches.QueueMessages;
    using Soccer.Core.Shared.Enumerations;
    using Soccer.Database.Matches.Commands;

    public class ProcessMatchEventConsumer : IConsumer<IMatchEventProcessedMessage>
    {
        private readonly IDynamicRepository dynamicRepository;

        public ProcessMatchEventConsumer(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task Consume(ConsumeContext<IMatchEventProcessedMessage> context)
        {
            var matchEvent = context?.Message?.MatchEvent;

            if (matchEvent != null)
            {
                var tasks = new List<Task>
                {
                    UpdateLiveMatchResult(matchEvent),
                    InsertTimeline(matchEvent)
                };

                await Task.WhenAll(tasks);
            }
        }

        private async Task UpdateLiveMatchResult(MatchEvent matchEvent)
        {
            if (matchEvent.IsLatest)
            {
                await dynamicRepository.ExecuteAsync(new UpdateLiveMatchResultCommand(matchEvent.MatchId, matchEvent.MatchResult, matchEvent.EventDate));
                await dynamicRepository.ExecuteAsync(new UpdateLiveMatchLastTimelineCommand(matchEvent.MatchId, matchEvent.Timeline, matchEvent.EventDate));
            }
        }

        private async Task InsertTimeline(MatchEvent matchEvent)
            => await dynamicRepository.ExecuteAsync(new InsertTimelineCommand(
                matchEvent.MatchId,
                matchEvent.Timeline,
                Language.en_US,
                matchEvent.EventDate));
    }
}
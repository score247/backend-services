namespace Soccer.EventProcessors.Matches.MatchEvents
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using MassTransit;
    using Soccer.Core.Matches.Events;
    using Soccer.Core.Matches.QueueMessages;
    using Soccer.Core.Matches.QueueMessages.MatchEvents;
    using Soccer.Core.Shared.Enumerations;
    using Soccer.Database.Matches.Commands;

    public class MatchEndEventConsumer : IConsumer<IMatchEndEventMessage>
    {
        private const byte DefaultMatchEndTime = 91;
        private static readonly ReadOnlyDictionary<PeriodType, byte> MatchEndTimes
          = new ReadOnlyDictionary<PeriodType, byte>(
              new Dictionary<PeriodType, byte>
              {
                  [PeriodType.RegularPeriod] = 91,
                  [PeriodType.Overtime] = 121,
                  [PeriodType.Penalties] = 122, // force it later than penalty_shootout
              });

        private readonly IBus messageBus;
        private readonly IDynamicRepository dynamicRepository;

        public MatchEndEventConsumer(IBus messageBus, IDynamicRepository dynamicRepository)
        {
            this.messageBus = messageBus;
            this.dynamicRepository = dynamicRepository;
        }

        public async Task Consume(ConsumeContext<IMatchEndEventMessage> context)
        {
            var matchEvent = context?.Message?.MatchEvent;

            if (matchEvent == null)
            {
                return;
            }

            var latestPeriodType = matchEvent.MatchResult.MatchPeriods.LastOrDefault()?.PeriodType;
            var matchTime = latestPeriodType != null && MatchEndTimes.ContainsKey(latestPeriodType) 
                                ? MatchEndTimes[latestPeriodType]
                                : DefaultMatchEndTime;

            matchEvent.Timeline.UpdateMatchTime(matchTime);

            await dynamicRepository.ExecuteAsync(new UpdateLiveMatchLastTimelineCommand(matchEvent.MatchId, matchEvent.Timeline));

            await messageBus.Publish<IMatchEventProcessedMessage>(new MatchEventProcessedMessage(matchEvent));
            await messageBus.Publish<ILiveMatchClosedMessage>(new LiveMatchClosedMessage(matchEvent?.MatchId, matchEvent?.MatchResult));
        }
    }
}
namespace Soccer.EventProcessors.Matches.MatchEvents
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using MassTransit;
    using Soccer.Core.Matches.QueueMessages;
    using Soccer.Core.Matches.QueueMessages.MatchEvents;
    using Soccer.Core.Shared.Enumerations;
    using Soccer.Database.Matches.Commands;

    public class PeriodStartEventConsumer : IConsumer<IPeriodStartEventMessage>
    {
        private const byte DefaultPeriodStartTime = 1;

        private static readonly ReadOnlyDictionary<Tuple<PeriodType, int>, byte> PeriodTimes
          = new ReadOnlyDictionary<Tuple<PeriodType, int>, byte>(
              new Dictionary<Tuple<PeriodType, int>, byte>
              {
                  [new Tuple<PeriodType, int>(PeriodType.RegularPeriod, 1)] = 1,
                  [new Tuple<PeriodType, int>(PeriodType.RegularPeriod, 2)] = 46,
                  [new Tuple<PeriodType, int>(PeriodType.Overtime, 1)] = 91,
                  [new Tuple<PeriodType, int>(PeriodType.Overtime, 2)] = 106,
                  [new Tuple<PeriodType, int>(PeriodType.Penalties, 0)] = 121,
              });

        private readonly IBus messageBus;
        private readonly IDynamicRepository dynamicRepository;

        public PeriodStartEventConsumer(IBus messageBus, IDynamicRepository dynamicRepository)
        {
            this.messageBus = messageBus;
            this.dynamicRepository = dynamicRepository;
        }

        public async Task Consume(ConsumeContext<IPeriodStartEventMessage> context)
        {
            var matchEvent = context?.Message?.MatchEvent;

            if (matchEvent == null)
            {
                return;
            }

            var period = new Tuple<PeriodType, int>(matchEvent.Timeline.PeriodType, matchEvent.Timeline.Period);
            var matchTime = PeriodTimes.ContainsKey(period) 
                ? PeriodTimes[period] 
                : DefaultPeriodStartTime;

            matchEvent.Timeline.UpdateMatchTime(matchTime);

            var command = new UpdateLiveMatchCurrentPeriodStartTimeCommand(matchEvent.MatchId, matchEvent.Timeline.Time);
            await dynamicRepository.ExecuteAsync(command);

            await messageBus.Publish<IMatchEventProcessedMessage>(new MatchEventProcessedMessage(matchEvent));
        }
    }
}
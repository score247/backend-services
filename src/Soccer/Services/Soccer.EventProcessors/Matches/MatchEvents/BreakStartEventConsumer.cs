using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MassTransit;
using Soccer.Core.Matches.QueueMessages;
using Soccer.Core.Matches.QueueMessages.MatchEvents;
using Soccer.Core.Shared.Enumerations;

namespace Soccer.EventProcessors.Matches.MatchEvents
{
    public class BreakStartEventConsumer : IConsumer<IBreakStartEventMessage>
    {
        private const byte DefaultBreakStartTime = 45;
        private static readonly ReadOnlyDictionary<PeriodType, byte> BreakTimes
          = new ReadOnlyDictionary<PeriodType, byte>(
              new Dictionary<PeriodType, byte>
              {
                  [PeriodType.Pause] = 45,
                  [PeriodType.AwaitingExtraTime] = 90,
                  [PeriodType.AwaitingPenalties] = 120,
              });

        private readonly IBus messageBus;

        public BreakStartEventConsumer(IBus messageBus)
        {
            this.messageBus = messageBus;
        }

        public async Task Consume(ConsumeContext<IBreakStartEventMessage> context)
        {
            var matchEvent = context?.Message?.MatchEvent;

            if (matchEvent != null)
            {
                var matchTime = BreakTimes.ContainsKey(matchEvent.Timeline.PeriodType) 
                    ? BreakTimes[matchEvent.Timeline.PeriodType] 
                    : DefaultBreakStartTime;

                matchEvent.Timeline.UpdateMatchTime(matchTime);

                await messageBus.Publish<IMatchEventProcessedMessage>(new MatchEventProcessedMessage(matchEvent));
            }
        }
    }
}

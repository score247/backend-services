using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Soccer.Core.Matches.QueueMessages;
using Soccer.Core.Matches.QueueMessages.MatchEvents;

namespace Soccer.EventProcessors.Matches.MatchEvents
{
    public class PenaltyEventConsumer : IConsumer<IPenaltyEventMessage>
    {
        private const byte DefaultPenaltyMatchTime = 121;

        private readonly IBus messageBus;

        public PenaltyEventConsumer(IBus messageBus)

        {
            this.messageBus = messageBus;
        }

        public async Task Consume(ConsumeContext<IPenaltyEventMessage> context)
        {
            var matchEvent = context?.Message?.MatchEvent;

            if (matchEvent == null)
            {
                return;
            }

            matchEvent.Timeline.UpdateMatchTime(DefaultPenaltyMatchTime);
            matchEvent.Timeline.ShootoutHomeScore = matchEvent.MatchResult.MatchPeriods
                .FirstOrDefault(period => period.PeriodType.IsPenalties())?.HomeScore ?? 0;
            matchEvent.Timeline.ShootoutAwayScore = matchEvent.MatchResult.MatchPeriods
                .FirstOrDefault(period => period.PeriodType.IsPenalties())?.AwayScore ?? 0;

            await messageBus.Publish<IMatchEventProcessedMessage>(new MatchEventProcessedMessage(matchEvent));
        }
    }
}
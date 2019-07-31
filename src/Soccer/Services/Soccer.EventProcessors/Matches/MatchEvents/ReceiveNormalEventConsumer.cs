namespace Soccer.EventProcessors.Matches.MatchEvents
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using MassTransit;
    using Soccer.Core.Shared.Enumerations;
    using Soccer.Core.Matches.Models;
    using Soccer.Core.Matches.QueueMessages;
    using Soccer.Core.Matches.QueueMessages.MatchEvents;

    public class ReceiveNormalEventConsumer : BaseMatchEventConsumer, IConsumer<INormalEventReceivedMessage>
    {
        private static readonly IDictionary<int, int> PeriodStartTimeMapper =
             new Dictionary<int, int>
             {
                        { MatchStatus.FirstHaft.Value, 1 },
                        { MatchStatus.SecondHaft.Value, 46 },
                        { MatchStatus.FirstHaftExtra.Value, 91 },
                        { MatchStatus.SecondHaftExtra.Value, 106 }
             };

        private readonly IBus messageBus;

        public ReceiveNormalEventConsumer(IBus messageBus, ICacheService cacheService, IDynamicRepository dynamicRepository)
            : base(cacheService, dynamicRepository)
        {
            this.messageBus = messageBus;
        }

        public async Task Consume(ConsumeContext<INormalEventReceivedMessage> context)
        {
            var matchEvent = context?.Message?.MatchEvent;

            if (await EventNotProcessed(matchEvent))
            {
                GenerateMatchTime(matchEvent);

                await messageBus.Publish<IMatchEventProcessedMessage>(new MatchEventProcessedMessage(matchEvent));
            }
        }

        private static void GenerateMatchTime(MatchEvent matchEvent)
        {
            if (matchEvent.MatchResult.EventStatus.IsLive())
            {
                var timeline = matchEvent.Timeline;
                var matchTime = timeline.MatchTime;
                var matchStatus = matchEvent.MatchResult.MatchStatus.Value;

                if (timeline.Type.IsPeriodStart && PeriodStartTimeMapper.ContainsKey(matchStatus))
                {
                    matchTime = PeriodStartTimeMapper[matchStatus];
                }

                if (!string.IsNullOrEmpty(timeline.StoppageTime))
                {
                    matchTime += int.Parse(timeline.StoppageTime);
                }

                if (matchTime > 0)
                {
                    matchEvent.MatchResult.MatchTime = matchTime;
                }
            }
        }
    }
}
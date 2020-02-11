using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using Fanex.Logging;
using MassTransit;
using Newtonsoft.Json;
using Soccer.Core.Matches.Models;
using Soccer.Core.Matches.QueueMessages.MatchEvents;
using Soccer.Database.Matches.Commands;

namespace Soccer.EventProcessors.Matches.MatchEvents
{
    public class InjuryTimeEventConsumer : IConsumer<IInjuryTimeEventMessage>
    {
        private readonly IDynamicRepository dynamicRepository;

        public InjuryTimeEventConsumer(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

#pragma warning disable S1541 // Methods and properties should not be too complex
        public async Task Consume(ConsumeContext<IInjuryTimeEventMessage> context)
#pragma warning restore S1541 // Methods and properties should not be too complex
        {
            var matchEvent = context?.Message?.MatchEvent;
            var matchStatus = matchEvent?.MatchResult?.MatchStatus;

            if (matchEvent == null
                || matchStatus == null
                || matchEvent.Timeline?.Type.IsInjuryTime() == false)
            {
                return;
            }

#pragma warning disable S1067 // Expressions should not be too complex

            var injuryTimes = new InjuryTimes(
                   IsRegularPeriod(matchEvent.Timeline, 1) ? (byte)matchEvent.Timeline.InjuryTimeAnnounced : (byte)0,
                   IsRegularPeriod(matchEvent.Timeline, 2) ? (byte)matchEvent.Timeline.InjuryTimeAnnounced : (byte)0,
                   IsOvertimePeriod(matchEvent.Timeline, 1) ? (byte)matchEvent.Timeline.InjuryTimeAnnounced : (byte)0,
                   IsOvertimePeriod(matchEvent.Timeline, 2) ? (byte)matchEvent.Timeline.InjuryTimeAnnounced : (byte)0
                );
#pragma warning restore S1067 // Expressions should not be too complex

            await dynamicRepository.ExecuteAsync(new UpdateLiveMatchInjuryTimesCommand(matchEvent.MatchId, injuryTimes, matchEvent.EventDate));
        }

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static
        private bool IsRegularPeriod(TimelineEvent timeline, int period)
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
        {
            return timeline.PeriodType.IsRegular() && timeline.Period == period;
        }

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static
        private bool IsOvertimePeriod(TimelineEvent timeline, int period)
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
        {
            return timeline.PeriodType.IsOvertime() && timeline.Period == period;
        }
    }
}

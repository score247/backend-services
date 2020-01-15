using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using MassTransit;
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
                   matchEvent.MatchResult.MatchStatus.IsFirstHalf() ? matchEvent.Timeline.InjuryTimeAnnounced : 0,
                   matchEvent.MatchResult.MatchStatus.IsSecondHalf() ? matchEvent.Timeline.InjuryTimeAnnounced : 0,
                   matchEvent.MatchResult.MatchStatus.IsFirstHaftExtra() ? matchEvent.Timeline.InjuryTimeAnnounced : 0,
                   matchEvent.MatchResult.MatchStatus.IsSecondHalfExtra() ? matchEvent.Timeline.InjuryTimeAnnounced : 0
                );
#pragma warning restore S1067 // Expressions should not be too complex

            await dynamicRepository.ExecuteAsync(new UpdateLiveMatchInjuryTimesCommand(matchEvent.MatchId, injuryTimes, matchEvent.EventDate));
        }
    }
}

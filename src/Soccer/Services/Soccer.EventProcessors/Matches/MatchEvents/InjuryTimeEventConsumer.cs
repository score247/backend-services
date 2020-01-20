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
        private readonly ILogger logger;

        public InjuryTimeEventConsumer(IDynamicRepository dynamicRepository, ILogger logger)
        {
            this.dynamicRepository = dynamicRepository;
            this.logger = logger;
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
            await logger.InfoAsync(string.Join(
                    " - ",
                    DateTime.Now,
                    $"{matchEvent?.MatchId}",
                    $"{matchEvent?.MatchResult?.MatchStatus?.DisplayName}",
                    $"{matchEvent?.Timeline?.Id}",
                    $"{matchEvent?.Timeline?.Type?.DisplayName}"));

            var injuryTimes = new InjuryTimes(
                   matchEvent.MatchResult.MatchStatus.IsFirstHalf() ? (byte)matchEvent.Timeline.InjuryTimeAnnounced : (byte)0,
                   matchEvent.MatchResult.MatchStatus.IsSecondHalf() ? (byte)matchEvent.Timeline.InjuryTimeAnnounced : (byte)0,
                   matchEvent.MatchResult.MatchStatus.IsFirstHaftExtra() ? (byte)matchEvent.Timeline.InjuryTimeAnnounced : (byte)0,
                   matchEvent.MatchResult.MatchStatus.IsSecondHalfExtra() ? (byte)matchEvent.Timeline.InjuryTimeAnnounced : (byte)0
                );
#pragma warning restore S1067 // Expressions should not be too complex

            await dynamicRepository.ExecuteAsync(new UpdateLiveMatchInjuryTimesCommand(matchEvent.MatchId, injuryTimes, matchEvent.EventDate));
        }
    }
}

namespace Soccer.DataReceivers.EventListeners.Matches
{
    using System.Threading.Tasks;
    using MassTransit;
    using Soccer.Core.Matches.Extensions;
    using Soccer.Core.Matches.QueueMessages.MatchEvents;
    using Soccer.DataProviders.Matches.Services;

    public interface IMatchEventListener
    {
        Task ListenMatchEvents();
    }

    public class MatchEventListener : IMatchEventListener
    {
        private readonly IBus messageBus;
        private readonly IMatchEventListenerService eventListenerService;

        public MatchEventListener(
            IBus messageBus,
            IMatchEventListenerService eventListenerService)
        {
            this.messageBus = messageBus;
            this.eventListenerService = eventListenerService;
        }

        public async Task ListenMatchEvents()
        {
            await eventListenerService.ListenEvents(async (matchEvent) =>
            {
                if (matchEvent.Timeline.IsScoreChangeInPenalty() && matchEvent.Timeline.IsShootOutInPenalty())
                {
                    await messageBus.Publish<IPenaltyEventReceivedMessage>(new PenaltyEventReceivedMessage(matchEvent));
                    return;
                }

                if (matchEvent.Timeline.Type.IsMatchEnd())
                {
                    await messageBus.Publish<IMatchEndEventReceivedMessage>(new MatchEndEventReceivedMessage(matchEvent));
                    return;
                }

                await messageBus.Publish<INormalEventReceivedMessage>(new NormalEventReceivedMessage(matchEvent));
            });
        }
    }
}
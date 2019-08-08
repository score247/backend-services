namespace Soccer.DataReceivers.EventListeners.Matches
{
    using System.Threading.Tasks;
    using MassTransit;
    using Soccer.Core.Matches.Extensions;
    using Soccer.Core.Matches.QueueMessages.MatchEvents;
    using Soccer.DataProviders.Matches.Services;
    using Soccer.DataReceivers.Odds;

    public interface IMatchEventListener
    {
        Task ListenMatchEvents();
    }

    public class MatchEventListener : IMatchEventListener
    {
        private readonly IBus messageBus;
        private readonly IMatchEventListenerService eventListenerService;
        private readonly IOddsMessagePublisher oddsMessagePublisher;

        public MatchEventListener(
            IBus messageBus,
            IMatchEventListenerService eventListenerService,
            IOddsMessagePublisher oddsMessagePublisher)
        {
            this.messageBus = messageBus;
            this.eventListenerService = eventListenerService;
            this.oddsMessagePublisher = oddsMessagePublisher;
        }

        public async Task ListenMatchEvents()
        {
            await eventListenerService.ListenEvents(async (matchEvent) =>
            {
                if (matchEvent.Timeline.IsScoreChangeInPenalty())
                {
                    return;
                }

                if (matchEvent.Timeline.IsShootOutInPenalty())
                {
                    await messageBus.Publish<IPenaltyEventReceivedMessage>(new PenaltyEventReceivedMessage(matchEvent));
                    return;
                }

                if (matchEvent.Timeline.Type.IsMatchEnd())
                {
                    await messageBus.Publish<IMatchEndEventReceivedMessage>(new MatchEndEventReceivedMessage(matchEvent));
                    return;
                }

                var normalEventMessage = new NormalEventReceivedMessage(matchEvent);
                
                await Task.WhenAll(
                    messageBus.Publish<INormalEventReceivedMessage>(normalEventMessage),
                    Task.Factory.StartNew(() => { oddsMessagePublisher.PublishOdds(normalEventMessage); }));
            });
        }
    }
}
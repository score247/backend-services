namespace Soccer.DataReceivers.EventListeners.Matches
{
    using System.Threading.Tasks;
    using MassTransit;
    using Soccer.Core.Matches.Events;
    using Soccer.DataProviders.Matches.Services;
    using Soccer.DataReceivers.EventListeners.Matches.MatchEventHandlers;

    public interface IMatchEventListener
    {
        Task ListenMatchEvents();
    }

    public class MatchEventListener : IMatchEventListener
    {
        private readonly IBus messageBus;
        private readonly IMatchEventListenerService eventListenerService;
        private readonly IMatchTimeHandler matchTimeHandler;
        private readonly IPenaltyEventHandler penaltyHandler;

        public MatchEventListener(
            IBus messageBus,
            IMatchEventListenerService eventListenerService,
            IMatchTimeHandler matchTimeHandler,
            IPenaltyEventHandler penaltyHandler)
        {
            this.messageBus = messageBus;
            this.eventListenerService = eventListenerService;
            this.matchTimeHandler = matchTimeHandler;
            this.penaltyHandler = penaltyHandler;
        }

        public async Task ListenMatchEvents()
        {
            await eventListenerService.ListenEvents(async (matchEvent) =>
            {
                matchTimeHandler.Handle(matchEvent);
                await penaltyHandler.Handle(matchEvent);

                await messageBus.Publish<IMatchEventsReceivedEvent>(new { MatchEvent = matchEvent });
            });
        }
    }
}
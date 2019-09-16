namespace Soccer.DataReceivers.EventListeners.Matches
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using MassTransit;
    using Microsoft.Extensions.Hosting;
    using Newtonsoft.Json;
    using Soccer.Core.Matches.QueueMessages;
    using Soccer.DataProviders.Matches.Services;
    using Soccer.DataReceivers.Odds;

    public class MatchEventListener : BackgroundService
    {
        private readonly IBus messageBus;
        private readonly IMatchEventListenerService eventListenerService;
        private readonly IOddsMessagePublisher oddsMessagePublisher;
        private readonly ILogger logger;

        public MatchEventListener(
            IBus messageBus,
            IMatchEventListenerService eventListenerService,
            IOddsMessagePublisher oddsMessagePublisher,
            ILogger logger)
        {
            this.messageBus = messageBus;
            this.eventListenerService = eventListenerService;
            this.oddsMessagePublisher = oddsMessagePublisher;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await logger.InfoAsync($"MatchEventListener is starting at {DateTime.Now}");

            await ListenMatchEvents(stoppingToken);

            await logger.InfoAsync($"MatchEventListener is stopping at {DateTime.Now}");
        }

        public async override Task StartAsync(CancellationToken cancellationToken)
        {
            await logger.InfoAsync($"Started Match Event Listener at {DateTime.Now}");

            await base.StartAsync(cancellationToken);
        }

        public async override Task StopAsync(CancellationToken cancellationToken)
        {
            await logger.InfoAsync($"Stop Match Event Listener at {DateTime.Now}");

            await base.StopAsync(cancellationToken);
        }

        private async Task ListenMatchEvents(CancellationToken stoppingToken)
        {
            await eventListenerService.ListenEvents(async (matchEvent) =>
            {
                 try
                 {
                     await Task.WhenAll(
                         messageBus.Publish<IMatchEventReceivedMessage>(new MatchEventReceivedMessage(matchEvent), stoppingToken),
                         oddsMessagePublisher.PublishOdds(matchEvent));
                 }
                 catch (Exception ex)
                 {
                     await logger.ErrorAsync(
                         string.Join(
                             "\r\n",
                             $"Match Event: {JsonConvert.SerializeObject(matchEvent)}",
                             $"Exception: {ex}"),
                         ex);
                 }
            }, stoppingToken);
        }
    }
}
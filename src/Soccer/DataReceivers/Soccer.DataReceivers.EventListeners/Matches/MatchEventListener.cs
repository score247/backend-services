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

    public class MatchEventListener : BackgroundService
    {
        private readonly IBus messageBus;
        private readonly IMatchEventListenerService eventListenerService;
        private readonly ILogger logger;

        public MatchEventListener(
            IBus messageBus,
            IMatchEventListenerService eventListenerService,
            ILogger logger)
        {
            this.messageBus = messageBus;
            this.eventListenerService = eventListenerService;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await logger.InfoAsync($"MatchEventListener {eventListenerService.Name} is starting at {DateTime.Now}");

            await ListenMatchEvents(stoppingToken);

            await logger.InfoAsync($"MatchEventListener {eventListenerService.Name} is stopping at {DateTime.Now}");
        }

        public async override Task StartAsync(CancellationToken cancellationToken)
        {
            await logger.InfoAsync($"Started Match Event Listener {eventListenerService.Name} at {DateTime.Now}");

            await base.StartAsync(cancellationToken);
        }

        public async override Task StopAsync(CancellationToken cancellationToken)
        {
            await logger.InfoAsync($"Stop Match Event Listener {eventListenerService.Name} at {DateTime.Now}");

            await base.StopAsync(cancellationToken);
        }

        private async Task ListenMatchEvents(CancellationToken stoppingToken)
        {
            await eventListenerService.ListenEvents(async (matchEvent) =>
            {
                try
                {
                    await messageBus.Publish<IMatchEventReceivedMessage>(new MatchEventReceivedMessage(matchEvent), stoppingToken);
                }
                catch (Exception ex)
                {
                    await logger.ErrorAsync(
                        string.Join(
                            "\r\n",
                            $"Match Event {eventListenerService.Name}: {JsonConvert.SerializeObject(matchEvent)}",
                            $"Exception: {ex}"),
                        ex);
                }
            }, stoppingToken);
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Soccer.Core.Leagues.Models;
using Soccer.Core.Matches.Models;
using Soccer.Core.Shared.Enumerations;
using Soccer.DataProviders.Leagues;

[assembly: InternalsVisibleTo("Soccer.DataReceivers.EventListeners.Tests")]

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
        private readonly ILeagueService internalLeagueService;

        public MatchEventListener(
            IBus messageBus,
            IMatchEventListenerService eventListenerService,
            ILogger logger,
            ILeagueService internalLeagueService)
        {
            this.messageBus = messageBus;
            this.eventListenerService = eventListenerService;
            this.logger = logger;
            this.internalLeagueService = internalLeagueService;
        }

        internal IEnumerable<League> MajorLeagues { get; set; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await logger.InfoAsync($"MatchEventListener {eventListenerService.Name} is starting at {DateTime.Now}");

            await ListenMatchEvents(stoppingToken);

            await logger.InfoAsync($"MatchEventListener {eventListenerService.Name} is stopping at {DateTime.Now}");
        }

        public async override Task StartAsync(CancellationToken cancellationToken)
        {
            MajorLeagues = await internalLeagueService.GetLeagues(Language.en_US);

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
            await eventListenerService.ListenEvents(
                async matchEvent => await HandleEvent(matchEvent, stoppingToken),
                stoppingToken);
        }

        protected internal async Task HandleEvent(MatchEvent matchEvent, CancellationToken cancellationToken)
        {
            try
            {
                if (MajorLeagues.Any(league => league.Id == matchEvent.LeagueId))
                {
                    await Task.WhenAll(
                        messageBus.Publish<IMatchEventReceivedMessage>(new MatchEventReceivedMessage(matchEvent), cancellationToken));
                }
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
        }
    }
}
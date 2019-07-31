namespace Soccer.DataProviders.SportRadar.Matches.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using Newtonsoft.Json;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Matches.Models;
    using Soccer.DataProviders.Matches.Services;
    using Soccer.DataProviders.SportRadar.Shared.Configurations;
    using Soccer.DataProviders.SportRadar.Shared.Extensions;
    using Soccer.DataProviders.SportRadar.Matches.DataMappers;
    using Soccer.DataProviders.SportRadar.Matches.Dtos;

    public class MatchEventListenerService : IMatchEventListenerService
    {
        private const int MillisecondsTimeout = 10 * 1000;
        private const string Endpoint = "soccer-xt3/{0}/stream/events/subscribe?format=json&api_key={1}";
        private readonly SportSettings soccerSettings;
        private readonly ISportRadarSettings sportRadarSettings;
        private readonly ILogger logger;

        public MatchEventListenerService(ISportRadarSettings sportRadarSettings, ILogger logger)
        {
            this.logger = logger;
            this.sportRadarSettings = sportRadarSettings;
            soccerSettings = sportRadarSettings.Sports.FirstOrDefault(s => s.Id == Sport.Soccer.Value);
        }

        public async Task ListenEvents(Action<MatchEvent> handler)
        {
            if (soccerSettings.Regions?.Any() == false)
            {
                return;
            }

            await ListenRegions(await GenerateStreamRegions(), handler);
        }

        private async Task<Dictionary<string, StreamReader>> GenerateStreamRegions()
        {
            var regionStreams = new Dictionary<string, StreamReader>();

            foreach (var region in soccerSettings.Regions)
            {
                if (!regionStreams.ContainsKey(region.Name))
                {
                    var formattedEndpoint = $"{sportRadarSettings.ServiceUrl}/{string.Format(Endpoint, region.Name, region.PushKey)}";
                    await logger.InfoAsync($"listen {formattedEndpoint}");
                    var endpoint = new Uri(formattedEndpoint);

                    var req = endpoint.CreateListenRequest();
                    var reply = (HttpWebResponse)req.GetResponse();
                    var stream = reply.GetResponseStream();
                    var reader = new StreamReader(stream);

                    regionStreams.Add(region.Name, reader);
                }
            }

            return regionStreams;
        }

        private async Task ListenRegions(Dictionary<string, StreamReader> regionStreams, Action<MatchEvent> handler)
        {
            while (true)
            {
                var tasks = regionStreams.Select(stream =>
                        Task.Factory.StartNew(async () => await ListenRegion(stream.Value, stream.Key, handler)));
                try
                {
                    await Task.WhenAll(tasks);
                }
                catch (AggregateException exception)
                {
                    foreach (var e in exception.InnerExceptions)
                    {
                        await logger.ErrorAsync($"{ DateTime.Now } Error while listening feed for Soccer", exception);
                    }

                    return;
                }

                // TODO: check again
                await Task.Delay(MillisecondsTimeout);
            }
        }

        private async Task ListenRegion(StreamReader reader, string region, Action<MatchEvent> handler)
        {
            string matchEventPayload;

            // TODO :  change do-while to while
            do
            {
                matchEventPayload = reader.ReadLine();
                await logger.InfoAsync($"{DateTime.Now} - region {region} Receiving: {matchEventPayload}");

                var matchEventDto = JsonConvert.DeserializeObject<MatchEventDto>(matchEventPayload);
                // TODO move json data to mapper
                var matchEvent = MatchMapper.MapMatchEvent(matchEventDto);

                handler.Invoke(matchEvent);
            } while (!string.IsNullOrWhiteSpace(matchEventPayload));
        }
    }
}
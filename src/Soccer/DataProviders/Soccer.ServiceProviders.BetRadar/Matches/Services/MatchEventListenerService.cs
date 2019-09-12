using System.Threading;

namespace Soccer.DataProviders.SportRadar.Matches.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using Score247.Shared.Enumerations;
    using Soccer.Core.Matches.Models;
    using Soccer.DataProviders.Matches.Services;
    using Soccer.DataProviders.SportRadar.Matches.DataMappers;
    using Soccer.DataProviders.SportRadar.Shared.Configurations;
    using Soccer.DataProviders.SportRadar.Shared.Extensions;

    public class MatchEventListenerService : IMatchEventListenerService
    {
        private const int MillisecondsTimeout = 10 * 1000;
        private readonly SportSettings soccerSettings;
        private readonly ISportRadarSettings sportRadarSettings;
        private readonly ILogger logger;
        private readonly Dictionary<string, StreamReader> regionStreams;

        public MatchEventListenerService(ISportRadarSettings sportRadarSettings, ILogger logger)
        {
            this.logger = logger;
            this.sportRadarSettings = sportRadarSettings;
            soccerSettings = sportRadarSettings.Sports.FirstOrDefault(s => s.Id == Sport.Soccer.Value);
            regionStreams = new Dictionary<string, StreamReader>();
        }

        public async Task ListenEvents(Action<MatchEvent> handler, CancellationToken cancellationToken)
        {
            if (soccerSettings?.Regions?.Any() == false)
            {
                return;
            }

            var tasks = soccerSettings?.Regions?.Select(region =>
                Task.Factory.StartNew(
                    () => ListeningEventForRegion(region.Name, region.PushKey, handler, cancellationToken),
                    cancellationToken));

            await Task.WhenAll(tasks);
        }

        public async Task ListeningEventForRegion(string regionName, string key, Action<MatchEvent> handler, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (!regionStreams.ContainsKey(regionName))
                {
                    var regionStream = await GenerateStreamRegion(regionName, key);

                    regionStreams.Add(regionStream.Key, regionStream.Value);

                    try
                    {
                        await ProcessStream(regionStream, handler);
                    }
                    catch (OperationCanceledException)
                    {
                        await logger.ErrorAsync($"Stream region '{regionName}' has been cancelled");
                    }
                    catch (Exception ex)
                    {
                        await logger.ErrorAsync($"Error while processing stream for region {regionName}. {ex}");
                    }
                    finally
                    {
                        regionStreams.Remove(regionName);
                    }
                }

                await Task.Delay(MillisecondsTimeout, cancellationToken);
            }
        }

        private async Task<KeyValuePair<string, StreamReader>> GenerateStreamRegion(string regionName, string key)
        {
            var formattedEndpoint = $"{sportRadarSettings.ServiceUrl}/" +
                $"{string.Format(sportRadarSettings.PushEventEndpoint, regionName, key)}";

            await logger.InfoAsync($"listen {formattedEndpoint}");

            var endpoint = new Uri(formattedEndpoint);

            var req = endpoint.CreateListenRequest();
            var reply = (HttpWebResponse)req.GetResponse();
            var stream = reply.GetResponseStream();
            var reader = new StreamReader(stream);

            return new KeyValuePair<string, StreamReader>(regionName, reader);
        }

        private async Task<string> ProcessStream(KeyValuePair<string, StreamReader> regionStream, Action<MatchEvent> handler)
        {
            var matchEventPayload = string.Empty;

            var reader = regionStream.Value;

            while (!reader.EndOfStream)
            {
                try
                {
                    matchEventPayload = reader.ReadLine();

                    var matchEvent = MatchMapper.MapMatchEvent(matchEventPayload);

                    if (matchEvent == default(MatchEvent))
                    {
                        continue;
                    }

                    handler.Invoke(matchEvent);

                    await logger.InfoAsync($"{DateTime.Now} - region {regionStream.Key} Receiving: {matchEventPayload}");
                }
                catch (Exception ex)
                {
                    await logger.ErrorAsync($"Message: {ex}\r\nPayload: {matchEventPayload}");
                }
            }

            return regionStream.Key;
        }
    }
}
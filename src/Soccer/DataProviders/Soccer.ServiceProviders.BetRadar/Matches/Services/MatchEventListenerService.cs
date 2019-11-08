namespace Soccer.DataProviders.SportRadar.Matches.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using Soccer.Core.Matches.Models;
    using Soccer.DataProviders.Matches.Services;
    using Soccer.DataProviders.SportRadar.Matches.DataMappers;
    using Soccer.DataProviders.SportRadar.Shared.Configurations;
    using Soccer.DataProviders.SportRadar.Shared.Extensions;

    public class MatchEventListenerService : IMatchEventListenerService
    {
        private const int MillisecondsTimeout = 10 * 1000;
        private const int FiveMinutes = 5;
        private const byte MaxRetryTimes = 5;
        private const int TimeDelayForStartingOtherEventListener = 500;
        private static byte retryCount;
        private readonly Region region;
        private readonly ILogger logger;
        private bool isWroteHeartbeatLog;
        private readonly ISportRadarSettings sportRadarSettings;

        public string Name { get; }

        public MatchEventListenerService(
            ISportRadarSettings sportRadarSettings,
            Region region,
            ILogger logger)
        {
            this.logger = logger;
            this.region = region;
            this.sportRadarSettings = sportRadarSettings;
            Name = region.Name;
        }

        public async Task ListenEvents(Action<MatchEvent> handler, CancellationToken cancellationToken)
        {
            if (region == null)
            {
                return;
            }

            try
            {
#pragma warning disable S2696 // Instance members should not write to "static" fields
                retryCount++;
#pragma warning restore S2696 // Instance members should not write to "static" fields
                await Task.Delay(TimeDelayForStartingOtherEventListener);
                await ListeningEventForRegion(region.Name, region.PushKey, handler, cancellationToken);
            }
            catch (AggregateException ex)
            {
                await logger.ErrorAsync($"Error in Start Listen Events {Name}: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                await logger.ErrorAsync($"Error in Start Listen Events {Name}: {ex.Message}", ex);
            }
            finally
            {
                if (retryCount < MaxRetryTimes)
                {
                    await Task.Delay(MillisecondsTimeout, cancellationToken);
                    await logger.InfoAsync($"Region: {region.Name}, Retry {retryCount} time for event listener at {DateTime.Now}");
                    await ListenEvents(handler, cancellationToken);
                }
            }
        }

        public async Task ListeningEventForRegion(string regionName, string key, Action<MatchEvent> handler, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await ProcessRegionStream(regionName, key, handler);

                await Task.Delay(MillisecondsTimeout, cancellationToken);

                await logger.InfoAsync($"{DateTime.Now} - Region {regionName} reconnect after end of stream");
            }
        }

        private async Task ProcessRegionStream(string regionName, string key, Action<MatchEvent> handler)
        {
            try
            {
                var regionStream = await GenerateStreamRegion(regionName, key);

                await ProcessStream(regionStream, handler);
            }
            catch (OperationCanceledException operationException)
            {
                await logger.ErrorAsync($"Stream region '{regionName}' has been cancelled", operationException);
            }
            catch (Exception ex)
            {
                await logger.ErrorAsync($"Error while processing stream for region {regionName}. {ex.Message}", ex);
            }
        }

        private async Task<KeyValuePair<string, StreamReader>> GenerateStreamRegion(string regionName, string key)
        {
            var formattedEndpoint = $"{sportRadarSettings.ServiceUrl}/" +
                $"{string.Format(sportRadarSettings.PushEventEndpoint, regionName, key)}";

            var endpoint = new Uri(formattedEndpoint);
            var req = endpoint.CreateListenRequest();
            var reply = (HttpWebResponse)req.GetResponse();
            var stream = reply.GetResponseStream();
            var reader = new StreamReader(stream);

            await logger.InfoAsync($"start listen {formattedEndpoint}");

            return new KeyValuePair<string, StreamReader>(regionName, reader);
        }

        private async Task ProcessStream(KeyValuePair<string, StreamReader> regionStream, Action<MatchEvent> handler)
        {
            var matchEventPayload = string.Empty;
            var reader = regionStream.Value;
            try
            {
                while (!reader.EndOfStream)
                {
                    matchEventPayload = reader.ReadLine();

                    var matchEvent = MatchMapper.MapMatchEvent(matchEventPayload);

                    await WriteHeartbeatLog(regionStream.Key);

                    if (matchEvent == default(MatchEvent))
                    {
                        continue;
                    }

                    handler.Invoke(matchEvent.AddScoreToSpecialTimeline(matchEvent.MatchResult));

                    await logger.InfoAsync($"{DateTime.Now} - region {regionStream.Key} Receiving: {matchEventPayload}");
                }
            }
            catch (Exception ex)
            {
                await logger.ErrorAsync($"Message: {ex}\r\nPayload: {matchEventPayload}");
            }
        }

        private async Task WriteHeartbeatLog(string region)
        {
            if (DateTime.Now.Minute % FiveMinutes == 0 && !isWroteHeartbeatLog)
            {
                await logger.InfoAsync($"Region {region} - Event Listener Heartbeat at {DateTime.Now}");
                isWroteHeartbeatLog = true;
            }

            if (DateTime.Now.Minute % FiveMinutes != 0)
            {
                isWroteHeartbeatLog = false;
            }
        }
    }
}
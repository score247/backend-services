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

namespace Soccer.DataProviders.SportRadar.Matches.Services
{
    public class MatchEventListenerService : IMatchEventListenerService
    {
        private const int MillisecondsTimeout = 10 * 1000;
        private const int DelayTime = 5000;
        private const byte MaxRetryTimes = 5;
        private const int TimeDelayForStartingOtherEventListener = 500;
        private static byte retryCount;
        private readonly Region region;
        private readonly ILogger logger;
        private readonly ISportRadarSettings sportRadarSettings;
        private readonly Dictionary<string, DateTime> healthCheckContainer;

        public string Name { get; }

        public MatchEventListenerService(
            ISportRadarSettings sportRadarSettings,
            Region region,
            ILogger logger,
            Dictionary<string, DateTime> healthCheckContainer)
        {
            this.logger = logger;
            this.region = region;
            this.sportRadarSettings = sportRadarSettings;
            Name = region.Name;
            this.healthCheckContainer = healthCheckContainer;
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
                cancellationToken.Register(() => logger.Error($"Region {Name} task was cancelled at {DateTime.Now}"));
                await Task.Delay(TimeDelayForStartingOtherEventListener, cancellationToken);
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

                    WriteHeartbeatLog(regionStream.Key);

                    if (matchEvent != default(MatchEvent))
                    {
                        await HandleEvent(regionStream, handler, matchEventPayload, matchEvent);
                    }
                    else
                    {
                        await Task.Delay(DelayTime);
                    }
                }

                await logger.ErrorAsync($"{DateTime.Now} - region {regionStream.Key} End of Stream");
            }
            catch (Exception ex)
            {
                await logger.ErrorAsync($"Message: {ex}\r\nPayload: {matchEventPayload}");
            }
        }

        private async Task HandleEvent(
            KeyValuePair<string, StreamReader> regionStream,
            Action<MatchEvent> handler, string matchEventPayload,
            MatchEvent matchEvent)
        {
            handler.Invoke(matchEvent.AddScoreToSpecialTimeline(matchEvent.MatchResult));

            if (sportRadarSettings.EnabledResponseLog)
            {
                await logger.InfoAsync($"{DateTime.Now} - region {regionStream.Key} Receiving: {matchEventPayload}");
            }
        }

        private void WriteHeartbeatLog(string region)
        {
            healthCheckContainer[region] = DateTime.Now;
        }
    }
}
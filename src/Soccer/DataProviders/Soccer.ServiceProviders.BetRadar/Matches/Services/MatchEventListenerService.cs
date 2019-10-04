namespace Soccer.DataProviders.SportRadar.Matches.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
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
        private static byte retryCount;
        private readonly SportSettings soccerSettings;
        private readonly ISportRadarSettings sportRadarSettings;
        private readonly ILogger logger;
        private readonly Dictionary<string, StreamReader> regionStreams;
        private bool isWroteHeartbeatLog;
        private IEnumerable<Task<Task>> runningTasks;

        public MatchEventListenerService(ISportRadarSettings sportRadarSettings, ILogger logger)
        {
            this.logger = logger;
            this.sportRadarSettings = sportRadarSettings;
            soccerSettings = sportRadarSettings.SoccerSettings;
            regionStreams = new Dictionary<string, StreamReader>();
        }

        public async Task ListenEvents(Action<MatchEvent> handler, CancellationToken cancellationToken)
        {
            if (soccerSettings?.Regions?.Any() == false)
            {
                return;
            }

            try
            {
#pragma warning disable S2696 // Instance members should not write to "static" fields
                retryCount++;
#pragma warning restore S2696 // Instance members should not write to "static" fields
                runningTasks = soccerSettings?.Regions?.Select(
                    region => Task.Factory.StartNew(
                                () => ListeningEventForRegion(region.Name, region.PushKey, handler, cancellationToken),
                    cancellationToken));

                await Task.WhenAll(runningTasks);
            }
            catch (AggregateException ex)
            {
                await logger.ErrorAsync($"Error in Start Listen Events: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                await logger.ErrorAsync($"Error in Start Listen Events: {ex.Message}", ex);
            }
            finally
            {
                if(retryCount < MaxRetryTimes)
                {
                    var tasksInformation = string.Join("\r\n", runningTasks.Select(runningTask => $"TaskId: {runningTask.Id}, Status: {runningTask.Status}"));
                    await Task.Delay(MillisecondsTimeout, cancellationToken);
                    await logger.InfoAsync($"Retry {retryCount} time for event listener at {DateTime.Now}.\r\nOld task info:{tasksInformation}");
                    await ListenEvents(handler, cancellationToken);
                }
            }
        }

        public async Task ListeningEventForRegion(string regionName, string key, Action<MatchEvent> handler, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (!regionStreams.ContainsKey(regionName))
                {
                    await ProcessRegionStream(regionName, key, handler);
                }

                await Task.Delay(MillisecondsTimeout, cancellationToken);

                await logger.InfoAsync($"{DateTime.Now} - Region {regionName} reconnect after end of stream");
            }
        }

        private async Task ProcessRegionStream(string regionName, string key, Action<MatchEvent> handler)
        {
            try
            {
                var regionStream = await GenerateStreamRegion(regionName, key);

                regionStreams.Add(regionStream.Key, regionStream.Value);

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
            finally
            {
                if (regionStreams.ContainsKey(regionName))
                {
                    regionStreams.Remove(regionName);
                }
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
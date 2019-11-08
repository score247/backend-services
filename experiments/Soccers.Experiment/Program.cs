namespace Soccers.Experiment
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using Fanex.Logging.Sentry;
    using Microsoft.AspNetCore.SignalR.Client;
    using Newtonsoft.Json;
    using Sentry;
    using Soccer.DataProviders.SportRadar.Matches.Services;
    using Soccer.DataProviders.SportRadar.Shared.Configurations;

    internal class Program
    {
        private static HubConnection connection;

        private static void Main(string[] args)
        {
            TestSignalRHubs();

            //StartListenStream();
        }

        private static void TestSignalRHubs()
        {
            connection = new HubConnectionBuilder()
                            .WithUrl("http://10.18.200.142:7872/dev/hubs/soccerhub")
                            .Build();

            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };

            connection.On<string>("MatchEvent", (data) =>
            {
                //var pushEvents = JsonConvert.DeserializeObject<Dictionary<string, MatchPushEvent>>(data);
                Console.WriteLine($"MatchEvent \r\n{data}\r\n ");
            });

            connection.On<string>("OddsMovement", (data) =>
            {
                //var pushEvents = JsonConvert.DeserializeObject<Dictionary<string, MatchPushEvent>>(data);
                Console.WriteLine($"OddsMovement \r\n{data}\r\n ");
            });

            connection.On<string>("OddsComparison", (data) =>
            {
                //var pushEvents = JsonConvert.DeserializeObject<Dictionary<string, MatchPushEvent>>(data);
                Console.WriteLine($"OddsComparison \r\n{data}\r\n ");
            });

            connection.On<string>("TeamStatistic", (data) =>
            {
                Console.WriteLine($"TeamStatistic \r\n{data}\r\n ");
            });

            connection.StartAsync().GetAwaiter().GetResult();

            Console.WriteLine("Start Listen");

            Console.ReadLine();
        }

        private static void StartListenStream()
        {
            Console.WriteLine("StartListenStream");

            var sportRadarSettings = new SportRadarSettings
            {
                ServiceUrl = "http://ha.nexdev.net:7206/V0/api/",
                PushEventEndpoint = "soccer-t3/{0}/stream/events/subscribe?format=json&api_key={1}",
                //ServiceUrl = "https://api.sportradar.us/",
                //PushEventEndpoint = "soccer-t3/{0}/stream/events/subscribe?format=json&api_key={1}",
                Sports = new List<SportSettings> {
                    new SportSettings {
                        Id= 1,
                        Name= "Soccer",
                        AccessLevel= "t",
                        Version= "3",
                        Regions= new List<Region>
                        {
                            new Region
                            {
                                  Name= "other",
                                  Key= "npc9md73nrwhykuepets3nqf",
                                  PushKey= "npc9md73nrwhykuepets3nqf"
                            },
                            new Region
                            {
                                  Name= "as",
                                  Key= "x3zffh29jgzbgz74nf6apvvy",
                                  PushKey= "x3zffh29jgzbgz74nf6apvvy"
                            }
                        }
                    }
                }
            };

            LogManager
                   .SetDefaultLogCategory("Score247-Event-Listeners-Demo-DEV-MACHINE")
                   .Use(new SentryLogging(new SentryEngineOptions
                   {
                       Dsn = new Dsn("https://c1354e53bd554c5185e97538260b0baa@sentry.nexdev.net/52")
                   }));

            foreach (var region in sportRadarSettings.SoccerSettings.Regions)
            {
                var eventListenerService = new MatchEventListenerService(sportRadarSettings, region, Logger.Log);

                Task.Run(() => eventListenerService.ListenEvents((matchEvent) =>
                {
                    try
                    {
                        Console.WriteLine(JsonConvert.SerializeObject(matchEvent));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(
                                string.Join(
                                "\r\n",
                                $"Match Event: {JsonConvert.SerializeObject(matchEvent)}",
                                $"Exception: {ex}"),
                                ex);
                    }
                }, default));
            }

            Console.ReadLine();
        }
    }
}
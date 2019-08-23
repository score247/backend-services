namespace Soccers.Experiment
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.SignalR.Client;

    internal class Program
    {
        private static HubConnection connection;

        private static void Main(string[] args)
        {
            connection = new HubConnectionBuilder()
                .WithUrl("https://score247-api2.nexdev.net/dev1/hubs/soccerhub")
                //.WithUrl("http://localhost:57321/hubs/Soccer/OddsEventHub")
                //.WithUrl("https://score247-api2.nexdev.net/dev/hubs/Soccer/OddsEventHub")
                .Build();

            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };

            connection.On<string>("OddsMovement", (data) =>
            {
                //var pushEvents = JsonConvert.DeserializeObject<Dictionary<string, MatchPushEvent>>(data);
                Console.WriteLine($"OddsMovement \r\n : {data}\r\n ");
            });

            connection.On<string>("OddsComparison", (data) =>
            {
                //var pushEvents = JsonConvert.DeserializeObject<Dictionary<string, MatchPushEvent>>(data);
                Console.WriteLine($"OddsComparison \r\n: {data}\r\n ");
            });

            connection.On<string>("OddsComparison", (data) =>
            {
                //var pushEvents = JsonConvert.DeserializeObject<Dictionary<string, MatchPushEvent>>(data);
                Console.WriteLine($"OddsComparison \r\n {data}");
            });

            connection.On<string>("TeamStatistic", (data) =>
            {
                Console.WriteLine($"TeamStatistic \r\n {data}");
            });

            connection.StartAsync().GetAwaiter().GetResult();

            Console.WriteLine("Start Listen");

            Console.ReadLine();
        }
    }
}
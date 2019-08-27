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
                .WithUrl("https://score247-api2.nexdev.net/test/hubs/Soccer/OddsEventHub")
                //.WithUrl("http://localhost:57321/hubs/Soccer/OddsEventHub")
                //.WithUrl("https://score247-api2.nexdev.net/dev/hubs/Soccer/OddsEventHub")
                .Build();

            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };

            connection.On<byte, string>("OddsMovement", (sportId, data) =>
            {
                //var pushEvents = JsonConvert.DeserializeObject<Dictionary<string, MatchPushEvent>>(data);
                Console.WriteLine($"OddsMovement \r\n {sportId}: {data}\r\n ");
            });

            connection.On<byte, string>("OddsComparison", (sportId, data) =>
            {
                //var pushEvents = JsonConvert.DeserializeObject<Dictionary<string, MatchPushEvent>>(data);
                Console.WriteLine($"OddsComparison \r\n {sportId}: {data}\r\n ");
            });

            connection.On<string>("TestEvent", (text) =>
            {
                Console.WriteLine(text);
            });

            connection.StartAsync().GetAwaiter().GetResult();

            Console.WriteLine("Start Listen");

            Console.ReadLine();
        }
    }
}
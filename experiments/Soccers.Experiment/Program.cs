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
                .WithUrl("http://localhost:57321/hubs/Soccer/OddsEventHub")
                .Build();

            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };

            connection.On<byte, string>("OddsMovement", (sportId, data) =>
            {
                //var pushEvents = JsonConvert.DeserializeObject<Dictionary<string, MatchPushEvent>>(data);
                Console.WriteLine($"OddsMovement \r\n {sportId}: {data}");
            });

            connection.On<byte, string>("OddsComparison", (sportId, data) =>
            {
                //var pushEvents = JsonConvert.DeserializeObject<Dictionary<string, MatchPushEvent>>(data);
                Console.WriteLine($"OddsComparison \r\n {sportId}: {data}");
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
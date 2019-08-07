namespace Soccer.EventPublishers.Matches.Hubs
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.SignalR;

    public class MatchEventHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            var client = Clients;

            return base.OnConnectedAsync();
        }
    }
}
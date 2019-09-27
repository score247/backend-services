namespace Soccer.EventPublishers.Hubs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.SignalR;

    public class SoccerHub : Hub
    {
        private static readonly HashSet<string> CurrentConnections = new HashSet<string>();

        public override Task OnConnectedAsync()
        {
            var connectionInfo = BuildConnectionInfo();
            if (!CurrentConnections.Contains(connectionInfo))
            {
                CurrentConnections.Add(connectionInfo);
            }

            return base.OnConnectedAsync();
        }

        private string BuildConnectionInfo()
            => $"Id: {Context.ConnectionId}, User Indentifier: {Context.UserIdentifier}";

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var connectionInfo = BuildConnectionInfo();
            var connection = CurrentConnections.FirstOrDefault(x => x == connectionInfo);

            if (connection != null)
            {
                CurrentConnections.Remove(connectionInfo);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public static List<string> GetAllActiveConnections()
            => CurrentConnections.ToList();
    }
}
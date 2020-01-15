using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.Logging;

namespace Soccer.DataReceivers.ScheduleTasks._Shared.HealthCheck
{
    public interface IHealthCheckService
    {
        Task HeartBeat(string name);

        Dictionary<string, DateTime> LatestHeartBeats();
    }

    public class HealthCheckService : IHealthCheckService
    {
        private readonly Dictionary<string, DateTime> healthCheckContainer;
        private readonly ILogger logger;

        public HealthCheckService(ILogger logger)
        {
            healthCheckContainer = new Dictionary<string, DateTime>();
            this.logger = logger;
        }

        public async Task HeartBeat(string name)
        {
            healthCheckContainer[name] = DateTime.Now;
            await logger.InfoAsync("HealthCheckService: " + name);
        }

        public Dictionary<string, DateTime> LatestHeartBeats() => healthCheckContainer;
    }
}
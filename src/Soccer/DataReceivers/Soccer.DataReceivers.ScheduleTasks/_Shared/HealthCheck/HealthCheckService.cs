using System;
using System.Collections.Generic;
using Fanex.Logging;

namespace Soccer.DataReceivers.ScheduleTasks._Shared.HealthCheck
{
    public interface IHealthCheckService
    {
        void HeartBeat(string name);

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

        public void HeartBeat(string name)
        {
            healthCheckContainer[name] = DateTime.Now;
            logger.Info("HealthCheckService: " + name);
        }

        public Dictionary<string, DateTime> LatestHeartBeats() => healthCheckContainer;
    }
}
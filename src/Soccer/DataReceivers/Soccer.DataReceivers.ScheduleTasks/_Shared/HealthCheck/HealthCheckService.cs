using System;
using System.Collections.Generic;

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

        public HealthCheckService()
        {
            healthCheckContainer = new Dictionary<string, DateTime>();
        }

        public void HeartBeat(string name)
        {
            healthCheckContainer[name] = DateTime.Now;
        }

        public Dictionary<string, DateTime> LatestHeartBeats() => healthCheckContainer;
    }
}
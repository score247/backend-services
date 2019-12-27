using System;
using System.Collections.Generic;
using System.Text;

namespace Soccer.DataReceivers.ScheduleTasks._Shared.HealthCheck
{
    public interface IHealthCheckService 
    {
        void HeartBeat(string name);

        Dictionary<string, DateTime> LatestHeartBeats();
    }

    public class HealthCheckService : IHealthCheckService
    {
        private readonly Dictionary<string, DateTime> healthcheckContainer;

        public HealthCheckService() 
        {
            healthcheckContainer = new Dictionary<string, DateTime>();
        }

        public void HeartBeat(string name)
        {
            if (healthcheckContainer.ContainsKey(name))
            {
                healthcheckContainer[name] = DateTime.Now;
            }
            else
            {
                healthcheckContainer.Add(name, DateTime.Now);
            }
        }

        public Dictionary<string, DateTime> LatestHeartBeats() => healthcheckContainer;
    }
}

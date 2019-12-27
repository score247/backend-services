using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Soccer.DataReceivers.ScheduleTasks._Shared.HealthCheck
{
    [Route("healthcheck")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        private const int maxExpiredMinutes = 5;
        private readonly IHealthCheckService healthCheckService;

        public HealthCheckController(IHealthCheckService healthCheckService)
        {
            this.healthCheckService = healthCheckService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var latestHeartbeat = healthCheckService.LatestHeartBeats();
            var jobsStatus = new List<(string, DateTime, bool)>();

            foreach (var heartbeat in latestHeartbeat)
            {
                jobsStatus.Add((heartbeat.Key, heartbeat.Value, heartbeat.Value.AddMinutes(maxExpiredMinutes) > DateTime.Now));
            }

            var statusCode = jobsStatus.All(job => job.Item3)
                ? HttpStatusCode.OK
                : HttpStatusCode.InternalServerError;

            return new ContentResult
            {
                ContentType = "text/html",
                Content = string.Join(
                            "<br />",
                            jobsStatus.Select(region => $"Job Name: {region.Item1} --- Last Health Check: {region.Item2} --- Is Alive: {region.Item3}")),
                StatusCode = (int)statusCode
            };
        }
    }
}

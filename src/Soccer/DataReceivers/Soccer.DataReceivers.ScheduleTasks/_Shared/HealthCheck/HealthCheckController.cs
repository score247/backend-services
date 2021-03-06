using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Fanex.Logging;
using Microsoft.AspNetCore.Mvc;

namespace Soccer.DataReceivers.ScheduleTasks._Shared.HealthCheck
{
    [Route("healthcheck")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        private const int maxExpiredMinutes = 5;
        private readonly IHealthCheckService healthCheckService;
        private readonly ILogger logger;

        public HealthCheckController(IHealthCheckService healthCheckService, ILogger logger)
        {
            this.healthCheckService = healthCheckService;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var latestHeartbeat = healthCheckService.LatestHeartBeats();
            var jobsStatus = latestHeartbeat
                .Select(heartbeat => (heartbeat.Key, heartbeat.Value, heartbeat.Value.AddMinutes(maxExpiredMinutes) > DateTime.Now))
                .ToList();

            var statusCode = jobsStatus.All(job => job.Item3) && jobsStatus.Count > 0
                ? HttpStatusCode.OK
                : HttpStatusCode.ServiceUnavailable;

            var content = string.Join(
                "<br />",
                jobsStatus.Select(region =>
                    $"Job Name: {region.Key} --- Last Health Check: {region.Value} --- Is Alive: {region.Item3}"));

            await logger.InfoAsync("Enter Health Check");

            return new ContentResult
            {
                ContentType = "text/html",
                Content = content,
                StatusCode = (int)statusCode
            };
        }
    }
}
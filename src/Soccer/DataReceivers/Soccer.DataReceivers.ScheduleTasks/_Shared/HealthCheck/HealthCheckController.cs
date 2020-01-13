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
            var jobsStatus = latestHeartbeat
                .Select(heartbeat => (heartbeat.Key, heartbeat.Value, heartbeat.Value.AddMinutes(maxExpiredMinutes) > DateTime.Now))
                .ToList();

            var statusCode = jobsStatus.All(job => job.Item3)
                ? HttpStatusCode.OK
                : HttpStatusCode.InternalServerError;

            return new ContentResult
            {
                ContentType = "text/html",
                Content = string.Join(
                            "<br />",
                            jobsStatus.Select(region => $"Job Name: {region.Key} --- Last Health Check: {region.Value} --- Is Alive: {region.Item3}")),
                StatusCode = (int)statusCode
            };
        }
    }
}
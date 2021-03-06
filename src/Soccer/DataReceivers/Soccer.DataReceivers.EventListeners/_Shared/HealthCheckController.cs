using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Soccer.DataReceivers.EventListeners.Shared
{
    [Route("healthcheck")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        private const int maxExpiredMinutes = 1;
        private readonly Dictionary<string, DateTime> healthCheckContainer;

        public HealthCheckController(Dictionary<string, DateTime> healthCheckContainer)
        {
            this.healthCheckContainer = healthCheckContainer;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var regions = healthCheckContainer
                .Select(healthCheckRegion
                    => (healthCheckRegion.Key, healthCheckRegion.Value, healthCheckRegion.Value.AddMinutes(maxExpiredMinutes) > DateTime.Now))
                .ToList();

            var statusCode = regions.All(region => region.Item3)
                ? HttpStatusCode.OK
                : HttpStatusCode.ServiceUnavailable;

            return new ContentResult
            {
                ContentType = "text/html",
                Content = string.Join(
                            "<br />",
                            regions.Select(region => $"Region Name: {region.Item1} --- Last Health Check: {region.Item2} --- Is Alive: {region.Item3}")),
                StatusCode = (int)statusCode
            };
        }
    }
}
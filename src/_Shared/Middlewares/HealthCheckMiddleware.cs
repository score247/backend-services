using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Score247.Shared.Middlewares
{
    public static class HealthCheckMiddleware
    {
        public static void AddHealthCheck(this IServiceCollection services)
        {
            var hcBuilder = services.AddHealthChecks();

            hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());
        }

        public static void UseHealthCheck(this IApplicationBuilder app)
        {
            app.UseHealthChecks("/healthStatus", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("self")
            });
        }
    }
}
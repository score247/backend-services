using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Soccer.NotificationPublishers._Shared.Configuration;

namespace Soccer.NotificationPublishers._Shared.Middlewares
{
    public static class CorsMiddleware
    {
        private const string CorsPolicy = "CorsPolicy";

        public static void AddCors(this IServiceCollection services, IAppSettings appSettings)
        {
            if (appSettings.AllowedCorsUrls.Any())
            {
                services.AddCors(c =>
                {
                    c.AddPolicy(CorsPolicy, builder =>
                    {
                        builder.WithOrigins(appSettings.AllowedCorsUrls.ToArray())
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    });
                });
            }
        }

        public static void AddCors(this IApplicationBuilder application)
        {
            application.UseCors(CorsPolicy);
        }
    }
}

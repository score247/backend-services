using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Soccer.NotificationPublisers.AppCenter._Shared.Configuration;
using Soccer.NotificationPublisers.AppCenter.Matches;
using Soccer.NotificationPublishers._Shared.Configuration;

namespace Soccer.NotificationPublishers._Shared.Middlewares
{
    public static class ServiceMiddleware
    {
        public static void AddServices(this IServiceCollection services, IConfiguration configuration, IAppSettings appSettings)
        {
            var appCenterSettings = new AppCenterSettings();
            configuration.GetSection("PushServices:AppCenter").Bind(appCenterSettings);
            services.AddSingleton<IAppCenterSettings>(appCenterSettings);

            services.AddSingleton(RestService.For<IPushApi>(appCenterSettings.ServiceUrl));
            services.AddSingleton<IMatchNotificationService, MatchNotificationService>();
        }
    }
}

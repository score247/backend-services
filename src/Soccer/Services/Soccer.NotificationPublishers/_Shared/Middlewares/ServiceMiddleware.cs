using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Soccer.NotificationPublishers._Shared.Configuration;
using Soccer.NotificationServices.AppCenter._Shared.Configuration;
using Soccer.NotificationServices.AppCenter.Matches;
using Soccer.NotificationServices.Matches;

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

using System;
using System.Threading.Tasks;
using Fanex.Logging;
using Refit;
using Soccer.Core.Notification.Models;
using Soccer.NotificationServices.AppCenter._Shared.Configuration;
using Soccer.NotificationServices.AppCenter.Dtos;
using Soccer.NotificationServices.AppCenter.Matches.DataMappers;
using Soccer.NotificationServices.Matches;

namespace Soccer.NotificationServices.AppCenter.Matches
{
    public interface IPushApi
    {
        [Post("/{ownerName}/{appName}/push/notifications")]
        Task<PushResultDto> Push([Header("X-API-Token")] string apiToken, string ownerName, string appname, [Body] PushDto push);
    }

    public class MatchNotificationService : IMatchNotificationService
    {
        private readonly IPushApi pushApi;
        private readonly ILogger logger;
        private readonly IAppCenterSettings settings;

        public MatchNotificationService(
            IAppCenterSettings settings,
            ILogger logger,
            IPushApi pushApi)
        {
            this.settings = settings;
            this.logger = logger;
            this.pushApi = pushApi;
        }

        public async Task<string> PushNotification(MatchEventNotification eventNotification)
        {
            var pushMessage = MatchNotificationMapper.MapPush(eventNotification, settings.DeviceTarget);

            try
            {
                var appName = eventNotification.IsIOS
                    ? settings.iOSAppName
                    : settings.AndroidAppName;

                var result = await pushApi.Push(settings.ApiKey, settings.Organization, appName, pushMessage);

                return result.notification_id;
            }
            catch (Exception ex)
            {
                if (ex is ApiException)
                {
                    await logger.ErrorAsync($"Match PushNotification Request Url {(ex as ApiException).Uri.ToString()}", ex);
                }

                await logger.ErrorAsync("Match PushNotification", ex);
            }

            return string.Empty;
        }
    }
}

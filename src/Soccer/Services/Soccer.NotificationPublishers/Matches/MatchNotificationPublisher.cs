using System;
using System.Threading.Tasks;
using Fanex.Logging;
using Refit;
using Soccer.Core.Notification.Models;
using Soccer.NotificationPublishers._Shared.Configuration;
using Soccer.NotificationPublishers._Shared.Dtos;
using Soccer.NotificationPublishers.Matches.DataMappers;

namespace Soccer.NotificationPublishers.Matches
{
    public interface IPushApi 
    {
        [Post("/{ownerName}/{appName}/push/notifications")]
        Task<PushResultDto> Push([Header("X-API-Token")] string apiToken, string ownerName, string appname, [Body] PushDto push);
    }

    public interface IMatchNotificationPublisher 
    {
        Task PushNotification(MatchEventNotification eventNotification);
    }

    public class MatchNotificationPublisher: IMatchNotificationPublisher
    {
        private readonly IPushApi pushApi;
        private readonly ILogger logger;
        private readonly IAppCenterSettings settings;

        public MatchNotificationPublisher(
            IAppCenterSettings settings, 
            ILogger logger,
            IPushApi pushApi) 
        {
            this.settings = settings;
            this.pushApi = pushApi;
            this.logger = logger;
        }
     
        public async Task PushNotification(MatchEventNotification eventNotification)
        {
            var pushMessage = MatchNotificationMapper.MapPush(eventNotification, settings.DeviceTarget);

            try
            {
                await pushApi.Push(settings.ApiKey, settings.Organizattion, settings.iOSAppName, pushMessage);
            }
            catch (Exception ex)
            {
                await logger.ErrorAsync("Match PushNotification", ex);
            }
        }
    }
}

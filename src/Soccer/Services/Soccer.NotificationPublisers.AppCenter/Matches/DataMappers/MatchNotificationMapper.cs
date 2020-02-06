using System.Collections.Generic;
using System.Linq;
using Soccer.Core._Shared.Enumerations;
using Soccer.Core.Notification.Models;
using Soccer.NotificationPublisers.AppCenter.Dtos;

namespace Soccer.NotificationPublisers.AppCenter.Matches.DataMappers
{
    public static class MatchNotificationMapper
    {
        public static PushDto MapPush(MatchEventNotification eventNotification, string deviceTarget)
        {
            var target = eventNotification.UserIds?.Any() == true
                   ? new TargetDto
                   {
                       type = deviceTarget,
                       devices = eventNotification.UserIds
                   }
                   : null;

            return new PushDto
            {
                notification_target = target,
                notification_content = new ContentDto
                {
                    name = nameof(MatchEventNotification),
                    title = eventNotification.Title,
                    body = eventNotification.Content,
                    custom_data = new Dictionary<string, string>
                    {
                        { nameof(eventNotification.MatchId), eventNotification.MatchId },
                        { nameof(eventNotification.SportId), eventNotification.SportId.ToString() },
                        { "sound", "default" },
                        { "type", NotificationType.MatchValue.ToString() }
                    }
                }
            };
        }
    }
}

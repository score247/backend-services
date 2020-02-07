using System.Collections.Generic;
using Soccer.Core._Shared.Enumerations;
using Soccer.Core.Notification.Models;
using Soccer.NotificationPublisers.AppCenter.Dtos;

namespace Soccer.NotificationPublisers.AppCenter.Matches.DataMappers
{
    public static class MatchNotificationMapper
    {
        public static PushDto MapPush(MatchEventNotification eventNotification, string deviceTarget)
        {
            var target = new TargetDto
            {
                type = deviceTarget,
                devices = eventNotification.UserIds
            };

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
                        { "id", eventNotification.MatchId },
                        { nameof(eventNotification.SportId), eventNotification.SportId.ToString() },
                        { "sound", "default" },
                        { "type", NotificationType.MatchValue.ToString() }
                    }
                }
            };
        }
    }
}

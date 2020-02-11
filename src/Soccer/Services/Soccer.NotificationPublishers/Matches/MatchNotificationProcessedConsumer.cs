using System.Threading.Tasks;
using MassTransit;
using Soccer.Core.Notification.QueueMessages;
using Soccer.NotificationServices.Matches;

namespace Soccer.NotificationPublishers.Matches
{
    public class MatchNotificationProcessedConsumer : IConsumer<IMatchNotificationProcessedMessage>
    {
        private readonly IMatchNotificationService notificationService;

        public MatchNotificationProcessedConsumer(IMatchNotificationService notificationService) 
        {
            this.notificationService = notificationService;
        }

        public async Task Consume(ConsumeContext<IMatchNotificationProcessedMessage> context)
        {
            var processedNotification = context.Message?.MatchNotification;

            if (processedNotification == null)
            {
                return;
            }

            await notificationService.PushNotification(processedNotification);
        }
    }
}

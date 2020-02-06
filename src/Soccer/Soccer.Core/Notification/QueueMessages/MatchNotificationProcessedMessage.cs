using Soccer.Core.Notification.Models;

namespace Soccer.Core.Notification.QueueMessages
{
    public interface IMatchNotificationProcessedMessage 
    {
        MatchEventNotification MatchNotification { get; }
    }

    public class MatchNotificationProcessedMessage : IMatchNotificationProcessedMessage
    {
        public MatchNotificationProcessedMessage(MatchEventNotification matchNotification)
        {
            MatchNotification = matchNotification;
        }

        public MatchEventNotification MatchNotification { get; }
    }
}

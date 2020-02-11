using System.Threading.Tasks;
using Soccer.Core.Notification.Models;

namespace Soccer.NotificationServices.Matches
{
    public interface IMatchNotificationService
    {
        Task<string> PushNotification(MatchEventNotification eventNotification);
    }
}
